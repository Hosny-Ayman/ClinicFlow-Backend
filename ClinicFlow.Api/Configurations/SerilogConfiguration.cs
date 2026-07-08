using Serilog;
using Serilog.Events;

public static class SerilogConfiguration
{
    public static void ConfigureSerilog(IConfiguration configuration)
    {
        var betterStackToken =
            configuration["betterStack:betterStackToken"];

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.WithProperty("Application", "ClinicFlow_API")
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .WriteTo.Console()
            .WriteTo.File(
                "Logs/log-.txt",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                buffered: false)
            .WriteTo.BetterStack(sourceToken: betterStackToken)
            .CreateLogger();
    }
}