using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Compute;
using Azure.ResourceManager.Resources;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Azure.ResourceManager.Resources.Models;

namespace CleanupFunctionApp
{
    /// <summary>
    /// Custom TokenCredential that performs interactive authentication using MSAL.NET.
    /// </summary>
    public class MsalInteractiveCredential : TokenCredential
    {
        // Instance of the MSAL public client application used for interactive authentication.
        private readonly IPublicClientApplication _clientApp;
        private readonly string[] _scopes;

        // Constructor to initialize the MsalInteractiveCredential class.
        public MsalInteractiveCredential(string clientId, string tenantId, string[] scopes)
        {
            // Store the requested scopes.
            _scopes = scopes;
            // Build the MSAL public client application instance.
            _clientApp = PublicClientApplicationBuilder.Create(clientId)
                .WithTenantId(tenantId) // Specify the Azure AD tenant ID.
                .WithRedirectUri("http://localhost") // Set the redirect URI for interactive login.
                .Build();
        }

        // Synchronous method to acquire an access token.
        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            // Call the asynchronous method and block until it completes.
            return GetTokenAsync(requestContext, cancellationToken).GetAwaiter().GetResult();
        }

        // Asynchronous method to acquire an access token.
        public override async ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            // Acquire token interactively
            // This will prompt the user to sign in via a browser window.
            var result = await _clientApp.AcquireTokenInteractive(_scopes)
                                         .ExecuteAsync(cancellationToken);
            return new AccessToken(result.AccessToken, result.ExpiresOn);
        }
    }

    /// <summary>
    /// Azure Function that cleans up unused resources based on a specified tag,
    /// using interactive authentication via MSAL.NET and sends email alerts.
    /// </summary>
    public class CleanupUnusedResources
    {
        private readonly ILogger _logger;
        private readonly string _subscriptionId;
        private readonly string _tagKey;
        private readonly string _clientId;
        private readonly string _tenantId;
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _emailRecipient;

        public CleanupUnusedResources(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CleanupUnusedResources>();

            // Read settings from environment variables (set in local.settings.json or App Settings)
            _subscriptionId = Environment.GetEnvironmentVariable("SubscriptionId") ?? throw new ArgumentNullException("SubscriptionId");
            _tagKey = Environment.GetEnvironmentVariable("TagKey") ?? "Test";
            _clientId = Environment.GetEnvironmentVariable("ClientId") ?? throw new ArgumentNullException("ClientId");
            _tenantId = Environment.GetEnvironmentVariable("TenantId") ?? throw new ArgumentNullException("TenantId");
           _smtpServer = Environment.GetEnvironmentVariable("SmtpServer") ?? throw new ArgumentNullException("SmtpServer");
            _smtpPort = int.Parse(Environment.GetEnvironmentVariable("SmtpPort") ?? throw new ArgumentNullException("SmtpPort"));
            _smtpUsername = Environment.GetEnvironmentVariable("SmtpUsername") ?? throw new ArgumentNullException("SmtpUsername");
            _smtpPassword = Environment.GetEnvironmentVariable("SmtpPassword") ?? throw new ArgumentNullException("SmtpPassword");
            _emailRecipient = Environment.GetEnvironmentVariable("EmailRecipient") ?? throw new ArgumentNullException("EmailRecipient");
        }

        // TimerTrigger set to run every 2 mins.
        [Function("CleanupUnusedResources")]
        public async Task Run([TimerTrigger("0 */2 * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"Cleanup function started at: {DateTime.Now}");

            // Define scopes for Azure Resource Manager.
            string[] scopes = new string[] { "https://management.azure.com/.default" };

            // Create an instance of the interactive credential.
            TokenCredential interactiveCredential = new MsalInteractiveCredential(_clientId, _tenantId, scopes);

            // Create an ArmClient using the interactive credential.
            var armClient = new ArmClient(interactiveCredential);

            // Get the subscription resource.
            var subscription = armClient.GetSubscriptionResource(new ResourceIdentifier($"/subscriptions/{_subscriptionId}"));

            // Iterate through each Resource Group in the subscription.
            await foreach (ResourceGroupResource rg in subscription.GetResourceGroups().GetAllAsync())
            {
                _logger.LogInformation($"Processing Resource Group: {rg.Data.Name}");

                // Iterate through all resources in the resource group.
                foreach (GenericResource resource in rg.GetGenericResources())
                {
                    if (resource.Data.Tags != null && resource.Data.Tags.TryGetValue("Environment", out var tagValue) && tagValue == "Test")
                    {
                        _logger.LogInformation($"Resource {resource.Data.Name} has the tag '{_tagKey}' with value '{tagValue}'. Deleting...");

                            _logger.LogInformation($"Deleting Resource: {resource.Data.Name} (Type: {resource.Data.ResourceType})");

                            // string userEmail = "dubemnkemka@gmail.com";
                            // string passWord = "tzdq xoqo qdfg odur";
                            // string receipientEmail = "dubzey007@gmail.com";

                        try
{
    // Delete the resource and wait for completion
    var deleteOperation = await resource.DeleteAsync(WaitUntil.Started);
    var response = await deleteOperation.WaitForCompletionResponseAsync();

    // Check if the deletion was successful (HTTP status code 200 or 204)
    if (response.Status == 200 || response.Status == 204)
    {
        _logger.LogInformation($"Deleted Resource: {resource.Data.Name} (Type: {resource.Data.ResourceType})");

        // Send email alert.
        await SendEmailAlertAsync(
                                    subject: $"Resource Deleted: {resource.Data.Name}",
                                    body: $"Resource {resource.Data.Name} of type {resource.Data.ResourceType} was deleted.",
                                    userEmail: _smtpUsername,
                                    userPassword: _smtpPassword,
                                    recipientEmail: _emailRecipient
                                );

    }
    else
    {
        _logger.LogError($"Failed to delete Resource '{resource.Data.Name}'. Status: {response.Status}");
    }
    }
    catch (Exception ex)
    {
        _logger.LogError($"Error deleting Resource '{resource.Data.Name}': {ex.Message}");
        _logger.LogError($"Stack Trace: {ex.StackTrace}");
    }
                _logger.LogInformation($"Cleanup function finished at: {DateTime.Now}");
                        }
                    }
                } 
            }

        // <summary>
        // Sends an email alert using SMTP.
        // </summary>
        private async Task SendEmailAlertAsync(string subject, string body, string userEmail, string userPassword, string recipientEmail)
        {
            try
            {
                using (var smtpClient = new SmtpClient(_smtpServer, _smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(userEmail, userPassword);
                    smtpClient.EnableSsl = true;
                    smtpClient.Timeout = 100000; 

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(userEmail),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = false
                    };
                    mailMessage.To.Add(recipientEmail);

                    await smtpClient.SendMailAsync(mailMessage);
                    _logger.LogInformation($"Email alert sent to {recipientEmail}");
                }
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError($"Error sending email alert: {smtpEx.Message}");
                _logger.LogError($"SMTP Status Code: {smtpEx.StatusCode}");
                _logger.LogError($"Inner Exception: {smtpEx.InnerException?.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email alert: {ex.Message}");
                _logger.LogError($"Inner Exception: {ex.InnerException?.Message}");
            }
        } 
    }
} 