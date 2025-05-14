# C# Stanbic Project - Group 5  
**A C# Azure Functions project demonstrating scheduled tasks using TimerTrigger**

## 📌 Overview  
This project showcases the use of **Azure Functions TimerTrigger** to execute scheduled tasks in C#. The core functionality includes a recurring function that runs every **5 minutes** using a cron expression. This template can be extended for scenarios like automated data processing, periodic cleanup tasks, or health checks.

---

## 🚀 Features  
- **Scheduled Execution**: Uses `TimerTrigger` to run functions on a predefined schedule  
- **Cron Expression**: Implements `0 */5 * * * *` to trigger tasks every 5 minutes  
- **Scalable Architecture**: Built on Azure Functions for serverless execution  
- **Reusability**: Easily adaptable for custom schedules or business logic  

---

## 🛠️ Prerequisites  
1. **.NET SDK**: Install [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0 ) (required for Azure Functions v4)  
2. **Azure Functions Core Tools**:  
   ```bash
   npm install -g azure-functions-core-tools@4
3. **Azure Account (Optional): For deploying to Azure**

## 📦 Installation & Setup
1. **Clone the repository**
- git clone https://github.com/dubemliveson/Csharp-Stanbicproject-group5.git 
- cd Csharp-Stanbicproject-group5

2. **Restore dependencies**
- dotnet restore

3. **Build the project**
- dotnet build

4. **Run the Azure Functions locally**
- func start

## 🕒 How It Works
**TimerTrigger Function**
The TimerTrigger function executes every 5 minutes using the cron expression 0 */5 * * * *.

**Cron Expression Breakdown**
| Field       | Value | Description                  |
|-------------|-------|------------------------------|
| Seconds     | `0`   | Trigger at 0 seconds         |
| Minutes     | `*/5` | Every 5 minutes              |
| Hours       | `*`   | Any hour                     |
| Day of Month| `*`   | Any day of the month         |
| Month       | `*`   | Any month                    |
| Day of Week | `*`   | Any day of the week          |

## 📝 Usage
1. Modify the TimerTrigger schedule in function.json or directly in the C# attribute
2. Replace the placeholder logic in Run() with your custom task
3. Test locally with func start, then deploy to Azure if needed

## 🌐 Deployment to Azure
1. Create a Function App in the Azure portal
2. Publish the project:

- func azure functionapp publish <Your-Function-App-Name>

## 🤝 Contributing
Contributions are welcome!

1. Fork the repository
2. Create a new branch (git checkout -b feature/your-feature)
3. Commit your changes (git commit -m 'Add feature')
4. Push to the branch (git push origin feature/your-feature)
5. Open a Pull Request
