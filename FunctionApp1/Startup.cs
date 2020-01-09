using FunctionApp1;
using FunctionApp1.Models;
using FunctionApp1.Processors;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[assembly: FunctionsStartup(typeof(Startup))]
namespace FunctionApp1
{
    public class Startup : FunctionsStartup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            // ------------------ default configuration initialise ------------------
            var serviceConfig = builder.Services.FirstOrDefault(s => s.ServiceType == typeof(IConfiguration));
            if (serviceConfig != null)
            {
                _ = (IConfiguration)serviceConfig.ImplementationInstance;
            }


            builder.Services.AddLogging();

           
            builder.Services.Configure<PhotoApiSettings>(Configuration.GetSection("PhotoApiSettings"));
            builder.Services.AddScoped<IPhotoProcessor, PhotoProcessor>();

        }
    }
}