using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs;
using Inventory_Management_System.Infrastructure.Services;
using Inventory_Management_System.Application.Features.ProductFeatures.Commands;

var host = Host.CreateDefaultBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        // Register Blob Storage Client
        services.AddSingleton(x =>
            new BlobServiceClient(Environment.GetEnvironmentVariable("AzureWebJobsStorage")));

        // Register PostgreSQL Database Context
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(Environment.GetEnvironmentVariable("DefaultConnection")));

        // Add Application Services (if needed)
        services.AddScoped<AddOrUpdateOrDeleteProduct>();

        // Optional: Enable Application Insights
        //services.AddApplicationInsightsTelemetryWorkerService();
    })
    .Build();

host.Run();
