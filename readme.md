## Stanbic Disep 4.0 Capstone Project - Group 5
<br>

## Azure Resource Cleanup Function  
A scheduled Azure Function for automated resource cleanup and email alerts

<br>

## 📌 Overview
This project implements an Azure Function that:

- Scans Azure resources tagged with `Environment=Test`
- Deletes matching resources automatically
- Sends email alerts via SMTP
- Uses MSAL.NET for interactive authentication
- Designed for development/testing environments to manage unused resources.

<br>

## 🚀 Features
- **Scheduled Cleanup:** Runs every 2 minutes via TimerTrigger
- **Tag-Based Filtering:** Targets resources with Environment=Test
- **Resource Deletion:** Deletes Azure resources programmatically
- **Email Notifications:** Sends alerts using SMTP
- **Interactive Auth:** Uses MSAL.NET for user-based Azure authentication
- **Comprehensive Logging:** Tracks execution details and errors

<br>
  
## 🛠️ Prerequisites
1. **.NET SDK:** .NET 8.0+
2. **Azure CLI:** Install here
3. **Azure Account:** With contributor access to target subscription
4. **SMTP Server:** Credentials for sending email alerts
5. **Environment Variables:** Configuration values (see below)

<br>
   
## 📦 Installation & Setup
**1. Clone Repository**
```bash
git clone https://github.com/dubemliveson/Csharp-Stanbicproject-group5.git 
cd Csharp-Stanbicproject-group5
```

**2. Configure Environment Variables**
Create a `local.settings.json` file with:
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "SubscriptionId": "your-subscription-id",
    "TagKey": "Environment",
    "ClientId": "your-app-registration-client-id",
    "TenantId": "your-directory-tenant-id",
    "SmtpServer": "smtp.yourprovider.com",
    "SmtpPort": "587",
    "SmtpUsername": "your-smtp-email@example.com",
    "SmtpPassword": "your-smtp-app-password",
    "EmailRecipient": "alert-recipient@example.com"
  }
}
```

**3. Install Dependencies**
```bash
dotnet restore
dotnet build
```

**4. Run Locally**
```bash
func start
```

## 🕒 How It Works

## ⏱️ Schedule
Uses cron expression `0 */2 * * * *` (every 2 minutes):
```csharp
[TimerTrigger("0 */2 * * * *")] TimerInfo myTimer
```

## 🔍 Resource Filtering
Deletes resources tagged with `Environment=Test`:
```csharp
if (resource.Data.Tags.TryGetValue("Environment", out var tagValue) && tagValue == "Test")
```

## 🗑️ Deletion Process
1. Authenticates via MSAL interactive flow
2. Iterates through all resource groups
3. Deletes matching resources
4. Waits for deletion completion
5. Logs status codes (200/204 = success)

## 📧 Email Alerts
Sends notifications via:
```csharp
await SendEmailAlertAsync(
    subject: $"Resource Deleted: {resource.Data.Name}",
    body: $"Resource {resource.Data.Name} of type {resource.Data.ResourceType} was deleted.",
    userEmail: _smtpUsername,
    userPassword: _smtpPassword,
    recipientEmail: _emailRecipient
);
```
<br>

## 📝 Usage
**1. Customize Tag Filter**
Modify tag key/value in `local.settings.json`:
```json
"TagKey": "Environment"
```

**2. Adjust Schedule**
Edit the TimerTrigger attribute:
```csharp
[TimerTrigger("0 */2 * * * *")] // Format: {second} {minute} {hour} {day} {month} {day-of-week}
```

**3. Configure SMTP**
Update email settings in `local.settings.json`

**4. Test Locally**
Run with `func start` and monitor logs

**5. Deploy to Azure**
```bash
az login
az functionapp create --name <your-app-name> --resource-group <group-name> --consumption-plan-location <location>
func azure functionapp publish <your-app-name>
```
<br>

## ⚠️ Security Notes
- **Interactive Auth:** Requires manual login each time (not suitable for production)
- **SMTP Credentials:** Use app passwords where possible
- **Production Alternative:** Use Managed Identities instead of interactive auth

 <br>
 
## 🌐 Deployment to Azure
**1. Create Function App**
```bash
az functionapp create \
  --name CleanupFunctionApp \
  --resource-group MyResourceGroup \
  --consumption-plan-location eastus \
  --runtime dotnet \
  --storage-account <storage-account-name>
```

**2. Publish Function**
```bash
func azure functionapp publish CleanupFunctionApp
```

**3. Configure App Settings**
Set environment variables in Azure Portal under "Configuration"

<br>

## 📊 Monitoring
Check Azure Monitor logs or local console output for:
- Resource deletion status
- Email delivery status
- Error details with stack traces

<br>

## 🤝 Contributing
1. Fork the repository
2. Create a feature branch (`git checkout -b feature/new-alert-channel`)
3. Commit changes (`git commit -m 'Add Slack alerts'`)
4. Push to branch (`git push origin feature/new-alert-channel`)
5. Open a pull request

📧 Contact
For questions or feedback:
- E-mail: dubemnkemka@gmail.com

**Built with ❤️ using Azure Functions, MSAL.NET, and C#**

📌 Additional Resources
- [Azure Functions Documentation](https://learn.microsoft.com/en-us/azure/azure-functions/?spm=a2ty_o01.29997173.0.0.1f8bc921NoIOAf)
- [MSAL.NET GitHub](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet?spm=a2ty_o01.29997173.0.0.1f8bc921NoIOAf)
- [Azure Resource Manager Docs](https://learn.microsoft.com/en-us/azure/azure-resource-manager/?spm=a2ty_o01.29997173.0.0.1f8bc921NoIOAf)

## 🧑‍🤝‍🧑 Contributors  
- **Chidubem Nkemka** - *Lead Developer* - [GitHub Profile](https://github.com/dubemliveson )  
- **Onyinyechi Anetoh** - *Cloud Engineer* - [GitHub Profile](https://github.com/janedoe )  
- **Ikechukwu Okoye** - *Architect* - [GitHub Profile](https://github.com/OluwaRuben )
- **Oluwasegun Gabriel** - *Architect* - [GitHub Profile](https://github.com/OluwaRuben )   

> Contributions are welcome! See [Contributing](#-contributing) for details.








