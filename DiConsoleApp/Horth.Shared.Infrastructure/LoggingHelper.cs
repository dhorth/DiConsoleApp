using Microsoft.Extensions.Configuration;
using Serilog;

namespace Horth.Shared.Infrastructure.Logger
{
    /// <summary>
    /// Setup Serilogger for this application
    /// </summary>
    public class LoggingHelper
    {
        public static ILogger CreateSerilogLogger(IConfiguration configuration, string appName)
        {
            var logger = new LoggerConfiguration()
                .Enrich.WithProperty("ApplicationContext", appName)
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(configuration);

            return logger.CreateLogger();
        }
    }
}
