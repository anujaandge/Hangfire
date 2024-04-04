# Hangfire CSV Generator
This project demonstrates how to use Hangfire to schedule recurring tasks for generating CSV files in a .NET application.

# Overview
Hangfire is a powerful library for .NET that enables background job processing, scheduling, and management. In this project, we utilize Hangfire to schedule a recurring task for generating CSV files. The CSV files contain data with a fixed schema, including fields such as "Name", "Age", "City", and "Date". The date for each record is provided as an input parameter to the CSV generation method.

# Features
Recurring CSV Generation: Utilizes Hangfire to schedule a recurring task for generating CSV files.
Dynamic Data Generation: Generates CSV files with dynamic data provided at runtime, including the current date.
Simple Configuration: Hangfire setup and job scheduling are straightforward and easily customizable.

# Usage:
1.Clone the repository to your local machine.
2.Open the solution in Visual Studio or your preferred IDE.
3.Build and run the application.
4.Access the Hangfire dashboard to monitor scheduled jobs and task execution.
5.To generate a CSV file with dynamic data:
   ->Call the GenerateCSVJob method with the desired file name and date.
   ->Provide the file name and date as parameters to the method.
# csharp code
    var myJobs = scope.ServiceProvider.GetRequiredService<MyJobs>();
    var fileName = "example.csv";
    var currentDate = DateTime.Today;
    myJobs.GenerateCSVJob(fileName, currentDate);
The generated CSV file will contain data with the specified file name and date.
# Dependencies
    Hangfire: Used for background job processing and scheduling.
    Microsoft.Extensions.DependencyInjection: Provides dependency injection support.
    Microsoft.Extensions.Hosting: Facilitates hosting services within the application.
    Microsoft.Extensions.Configuration: Enables configuration settings for the application.
# Configuration
    Ensure the connection string for Hangfire's storage (e.g., SQL Server) is configured in appsettings.json.
    Customize the GenerateCSVJob method according to your data schema and requirements.
     
