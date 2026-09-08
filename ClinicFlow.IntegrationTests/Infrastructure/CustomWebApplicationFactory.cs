using ClinicFlow.Application.Common.Interfaces;
using ClinicFlow.Application.Common.Interfaces.Jobs;
using ClinicFlow.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.IO;
using System.Linq;

namespace ClinicFlow.IntegrationTests.Infrastructure
{
    public class CustomWebApplicationFactory : WebApplicationFactory<ClinicFlow.Api.Program>
    {
        public CustomWebApplicationFactory()
        {
            var existingConnectionString =
                Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

            if (!string.IsNullOrWhiteSpace(existingConnectionString))
                return;

            var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

            var config = new ConfigurationBuilder()
                .AddJsonFile(path, optional: false)
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection");

            Environment.SetEnvironmentVariable( "ConnectionStrings__DefaultConnection", connectionString);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

                config.AddJsonFile(
                    path,
                    optional: false,
                    reloadOnChange: false);

                config.AddEnvironmentVariables();
            });

            builder.ConfigureServices((context, services) =>
            {
                var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

                if (string.IsNullOrEmpty(connectionString) || !connectionString.Contains("IntegrationTests"))
                {
                    throw new Exception("SAFETY GUARD: Integration tests must use a connection string containing 'IntegrationTests' to prevent accidental destruction of development data.");
                }

                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseSqlServer(connectionString);
                });

                var rateLimitDescriptors = services.Where(d => d.ServiceType == typeof(Microsoft.Extensions.Options.IConfigureOptions<Microsoft.AspNetCore.RateLimiting.RateLimiterOptions>)).ToList();
                foreach (var rd in rateLimitDescriptors)
                {
                    services.Remove(rd);
                }
                
                services.Configure<Microsoft.AspNetCore.RateLimiting.RateLimiterOptions>(options =>
                {
                    options.AddPolicy("LoginPolicy", ctx => 
                        System.Threading.RateLimiting.RateLimitPartition.GetFixedWindowLimiter("LoginPolicy", _ => new System.Threading.RateLimiting.FixedWindowRateLimiterOptions { PermitLimit = int.MaxValue, Window = TimeSpan.FromSeconds(1) })
                    );
                });

                var fileStorageDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IFileStorageService));
                if (fileStorageDescriptor != null)
                {
                    services.Remove(fileStorageDescriptor);
                }
                var mockFileStorage = new Mock<IFileStorageService>();
                services.AddSingleton(mockFileStorage.Object);

                var jobDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IDoctorVacationJobService));
                if (jobDescriptor != null)
                {
                    services.Remove(jobDescriptor);
                }
                var mockJobService = new Mock<IDoctorVacationJobService>();
                services.AddSingleton(mockJobService.Object);
            });
        }
        
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", null);
        }
    }
}
