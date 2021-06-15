using System;
using System.Threading.Tasks;
using Horth.Service.Email.Shared.Configuration;
using Horth.Shared.Infrastructure.Configuration;
using Horth.Shared.Infrastructure.Logger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Horth.Shared.Infrastructure.Console
{
    /// <summary>
    /// This is where the magic is obscured from the main application
    /// </summary>
    public class ConsoleHelper
    {
        /// <summary>
        /// I use this in all my console apps to setup the environment
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        public static IConfiguration InitializeConsole(string appName)
        {
            var config = ConfigurationHelper.GetConfiguration();
            Log.Logger = LoggingHelper.CreateSerilogLogger(config, appName);
            Log.Logger.Information($"Starting {appName} application");
            return config;

        }

        /// <summary>
        /// This is where the magic happens
        /// Configure Service setups up the hostbuilder
        /// adds our shared services (ie AppSettings)
        /// then adds any services passed in from the main application
        /// Adds a IConsoleApplication service using the implementation passed to us
        /// Then runs that implenations
        /// </summary>
        /// <typeparam name="T">The local implemenation of IConsoleApplication</typeparam>
        /// <param name="config">The IConfig loaded earlier</param>
        /// <param name="configureDelegate">delegate to run, that will add custom services</param>
        /// <returns>standard int return code</returns>
        public static async Task<int> ConfigureServices<T>(IConfiguration config, Action<HostBuilderContext, IServiceCollection> configureDelegate) where T : ConsoleApplication
        {
            var hostBuilder = new HostBuilder();
            hostBuilder.ConfigureServices((hostContext, services) =>
            {
                //Add our core services
                services.AddSingleton(config);
                services.AddSingleton<AppSettings>();

                //Add the services based in from the delegate
                configureDelegate(hostContext, services);

                //Add the IConsoleApplication service
                services.AddSingleton<IConsoleApplication, T>();
            });

            var host = hostBuilder.Build();
            var app=host.Services.GetService<IConsoleApplication>();
            var ret=await app.Run();
            return ret;
        }
    }


}
