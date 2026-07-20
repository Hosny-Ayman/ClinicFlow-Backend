using ClinicFlow.Api.Extensions;
using ClinicFlow.Application.Extensions;
using ClinicFlow.Infrastructure.Data;
using ClinicFlow.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ClinicFlow.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {


            var builder = WebApplication.CreateBuilder(args);

            SerilogConfiguration.ConfigureSerilog(builder.Configuration);

            builder.Host.UseSerilog();

            Log.Information("Program Work Good");

            var ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddCorsServices(builder.Configuration);

            builder.Services.AddApiServices(builder.Configuration);

            builder.Services.AddJwtAuthentication(builder.Configuration);
           
            builder.Services.AddRateLimitingServices();

            builder.Services.AddAuthorizationServices();

            builder.Services.AddApplicationServices();

            builder.Services.AddInfrastructureServices();

            builder.Services.AddInfrastructureServices(builder.Configuration);

            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(ConnectionString));
            

            var app = builder.Build();

          
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("ClinicFlowCorsPolicy");

            app.UseHttpsRedirection();

            app.UseApplicationMiddleware();

            app.UseAuthentication();

            app.UseRateLimiter();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
