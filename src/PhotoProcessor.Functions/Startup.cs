using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhotoProcessor.Functions;
using PhotoProcessor.Functions.Data;
using PhotoProcessor.Functions.Models;
using PhotoProcessor.Functions.Services;
using System;
using System.Linq;

[assembly: FunctionsStartup(typeof(Startup))]
namespace PhotoProcessor.Functions
{
    public class Startup : FunctionsStartup
    {

        public override void Configure(IFunctionsHostBuilder builder)
        {
            // ------------------ default configuration initialise ------------------
            var serviceConfig = builder.Services.FirstOrDefault(s => s.ServiceType == typeof(IConfiguration));
            if (serviceConfig != null)
            {
                _ = (IConfiguration)serviceConfig.ImplementationInstance;
            }

            builder.Services.AddLogging();

            // ------------------ TableStorageDb initialise ------------------
            ITableConfiguration tableConfig = new TableConfiguration
            {
                ConnectionString = Environment.GetEnvironmentVariable("StorageConnectionString"),
                TableName = Environment.GetEnvironmentVariable("TableName")
            };

            builder.Services.AddSingleton(tableConfig);
            builder.Services.AddSingleton<ITableDbContext, TableDbContext>();

            builder.Services.AddScoped<IBlobContext, BlobContext>();

            builder.Services.AddScoped<IDataRepository, DataRepository>();

            IPhotoApiSettings photoApiSettings = new PhotoApiSettings
            {
                PrivateKey = Environment.GetEnvironmentVariable("PhotoApiSettingsPrivateKey"),
                AppId = Environment.GetEnvironmentVariable("PhotoApiSettingsAppId")

            };

            builder.Services.AddSingleton(photoApiSettings);
            builder.Services.AddScoped<IPhotoService, PhotoFiddler>();
            builder.Services.AddScoped<IDownloadService, DownloadService>();

        }
    }
}