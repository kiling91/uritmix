using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.SystemConsole.Themes;

namespace Helpers.WebApi.Extensions;

public static class SerilogLoggingExtensions
{
    public static void AddSerilogLogging(this IApplicationBuilder app)
    {
        var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
        var log = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler",
                LogEventLevel.Fatal)
            .MinimumLevel.Override("Microsoft.Hosting", LogEventLevel.Information)
            .MinimumLevel.Override("Serilog", LogEventLevel.Warning)
            .Enrich.WithExceptionDetails()
            .Enrich.FromLogContext()
            .WriteTo.Console(
                outputTemplate: "{Timestamp:HH:mm:ss} [{Level}] {SourceContext} {Message}{NewLine}{Exception}",
                theme: AnsiConsoleTheme.Code
            )
            .CreateLogger();

        loggerFactory.AddSerilog(log);
        Log.Logger = log;
        var url = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
        Log.Logger.Information("Service start: {Url}", url);
    }
}