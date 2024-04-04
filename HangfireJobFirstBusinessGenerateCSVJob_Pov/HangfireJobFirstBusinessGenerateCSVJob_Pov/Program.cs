using Hangfire;
using Hangfire.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.DependencyInjection;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.Metrics;
using Hangfire.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add Hangfire services
builder.Services.AddHangfire(configuration => configuration.UseSqlServerStorage("Server=(LocalDB)\\MSSQLLocalDB;Database=Test1;Trusted_Connection=True;MultipleActiveResultSets=true;"));
builder.Services.AddHangfireServer();
builder.Services.AddScoped<MyJobs>();
builder.Services.AddScoped<ICSVGenerator, CSVGenerator>();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHangfireDashboard();
});

// Configure jobs using an instance of MyJobs obtained from the service provider
using (var scope = app.Services.CreateScope())
{
    var myJobs = scope.ServiceProvider.GetRequiredService<MyJobs>();
    myJobs.ConfigureJobs();
}

app.Run();

public interface ICSVGenerator
{
    void GenerateCSV(string fileName, IEnumerable<string[]> data);
}

public class CSVGenerator : ICSVGenerator
{
    public void GenerateCSV(string fileName, IEnumerable<string[]> data)
    {
        using (var writer = new StreamWriter(fileName))
        {
            foreach (var rowData in data)
            {
                writer.WriteLine(string.Join(",", rowData));
            }
        }
    }
}
public class MyJobs
{
    private readonly IRecurringJobManager _recurringJobManager;
    private readonly IServiceProvider _serviceProvider;

    public MyJobs(IRecurringJobManager recurringJobManager, IServiceProvider serviceProvider)
    {
        _recurringJobManager = recurringJobManager;
        _serviceProvider = serviceProvider;
    }

    public void ConfigureJobs()
    {
        _recurringJobManager.AddOrUpdate(
            "MonthlyBusinessDayJob",
            () => ExecuteMonthlyBusinessDayJob(),
            "0 8 1 * *", // First day of every month at 08:00 AM
            TimeZoneInfo.Local
        );

        // ... other normal jobs ...
        RecurringJob.AddOrUpdate(
            "HourlyJob",
            () => ExecuteHourlyJob(),
            Cron.Hourly, // Every hour
            TimeZoneInfo.Local
        );

        // Every Minuit job
        RecurringJob.AddOrUpdate(
            "EveryMinJob",
            () => ExecuteEveryMinuitJob(),
            "*/1 * * * *", // Every Min 
            TimeZoneInfo.Local
        );
        // Schedule the CSV generation job
        _recurringJobManager.AddOrUpdate(
            "GenerateCSVJob",
            () => GenerateCSVJob(@"C:\Users\andge\Desktop\.Net\HangfireJobFirstBusinessGenerateCSVJob_Pov\HangfireJobFirstBusinessGenerateCSVJob_Pov\demo.csv", DateTime.Today.AddDays(-7)),
            "*/1 * * * *",
            TimeZoneInfo.Local
        //Cron.Daily // You can adjust the frequency as needed
        );
    }

    public void ExecuteMonthlyBusinessDayJob()
    {
        DateTime today = DateTime.Today;

        if (IsBusinessDay(today))
        {
            Console.WriteLine("Executing monthly job on a business day: " + today.ToShortDateString());
            // Execute your job logic here

            // Reset the schedule for the next first day of the month
            ResetMonthlyJobSchedule();
        }
        else
        {
            Console.WriteLine("Not a business day, scheduling for the next day: " + today.AddDays(1).ToShortDateString());
            _recurringJobManager.AddOrUpdate(
                "MonthlyBusinessDayJob",
                () => ExecuteMonthlyBusinessDayJob(),
                $"{today.AddDays(1):HH mm * * *}", // Reschedule for the next day
                TimeZoneInfo.Local
            );
        }
    }

    private void ResetMonthlyJobSchedule()
    {
        _recurringJobManager.AddOrUpdate(
            "MonthlyBusinessDayJob",
            () => ExecuteMonthlyBusinessDayJob(),
            "0 8 1 * *", // Reset to first day of every month at 08:00 AM
            TimeZoneInfo.Local
        );
    }

    private bool IsBusinessDay(DateTime date)
    {
        // Check for business day logic here
        return date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday;
        // Extend this to check for public holidays
    }

    public void ExecuteHourlyJob()
    {
        Console.WriteLine("Executing hourly job at: " + DateTime.Now.ToShortTimeString());
        // Execute your hourly job logic here
    }

    public void ExecuteEveryMinuitJob()
    {

        Console.WriteLine("Executing ever min job: " + DateTime.Now.ToShortTimeString());
        // Execute your every min job logic here

    }
    public void GenerateCSVJob(string fileName, DateTime date)
    {



        var data = new List<string[]>
        {
            new string[] {"Name", "Age", "City", "Date"},
            new string[] {"Tom","20","Delhi",date.ToString()},
            new string[] {"John", "30", "New York",date.ToString()},
            new string[] {"Alice", "25", "Los Angeles", date.ToString()},
            new string[] {"Bob", "35", "Chicago", date.ToString()}
        };

        using (var scope = _serviceProvider.CreateScope())
        {
            var csvGenerator = scope.ServiceProvider.GetRequiredService<ICSVGenerator>();
            try
            {
                csvGenerator.GenerateCSV(fileName, data);
                Console.WriteLine($"CSV file '{fileName}' has been generated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating CSV file '{fileName}': {ex.Message}");
            }
        }

    }
}



//Create Test Database so Hangfire can create its tables
///CREATE DATABASE Test;


//To delete HangFire table if you have existing tables or data to be deleted for a fresh start
//drop table[Test].[HangFire].AggregatedCounter
//drop table [Test].[HangFire].Counter
//drop table [Test].[HangFire].Hash
//drop table [Test].[HangFire].Job
//drop table [Test].[HangFire].JobParameter
//drop table [Test].[HangFire].JobQueue
//drop table [Test].[HangFire].List
//drop table [Test].[HangFire].[Schema]
//drop table[Test].[HangFire].Server
//drop table [Test].[HangFire].[Set]
//drop table[Test].[HangFire].State