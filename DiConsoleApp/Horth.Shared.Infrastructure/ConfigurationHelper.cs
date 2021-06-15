using Microsoft.Extensions.Configuration;
using Serilog;
using System.IO;

namespace Horth.Shared.Infrastructure.Configuration
{

    /// <summary>
    /// Load the configuration file(s) for this application
    /// </summary>
    public static class ConfigurationHelper
    {
        public static IConfiguration GetConfiguration()
        {
            var env = "Production";
#if DEBUG
            env = "Development";
#endif

            if (File.Exists("appsettings.secrets.json"))
                Log.Logger.Debug("Using appsettings.secrets.json");

            if (File.Exists("appsettings.Shared.json"))
                Log.Logger.Debug("Using appsettings.shared.json");

            if (File.Exists($"appsettings.{env}.json"))
                Log.Logger.Debug($"Using appsettings.{ env}.json");

            if (File.Exists($"appsettings.Shared.{env}.json"))
                Log.Logger.Debug($"Using appsettings.Shared.{ env}.json");

            var builder = new ConfigurationBuilder()
                //.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Secrets.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.Shared.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.Shared.{env}.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }
    }
}
