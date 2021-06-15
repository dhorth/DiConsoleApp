using Horth.Shared.Infrastructure;
using Horth.Shared.Infrastructure.Console;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Threading.Tasks;

namespace DiConsoleApp
{
    /// <summary>
    /// Sample application for true dependency injection in a console application
    /// </summary>
    class Program
    {

        public static readonly string AppName = "Horth.Service.Email.TestClient";

        /// <summary>
        /// Using some helper fascade classes I'm going to setup a simple console 
        /// application Main function
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static async Task<int> Main(string[] args)
        {

            //Get the configuration and starts the logging
            var config = ConsoleHelper.InitializeConsole(AppName);

            //setup our DI, the base class will add our configuration, service registry etc
            //look at ConfigureServices, I setup shared service in that method so I don't need 
            //to keep adding for each application (ie AppSettings)
            //The ConsoleHelper ConfigureService, will load the services and call Run on our new
            //ConsoleApplication class
            var ret = await ConsoleHelper.ConfigureServices<TestClient>(config, (hostContext, service) =>
            {
                //Add all the services my application will need
                service.AddSingleton<IDummyService,DummyService>();
            });
            return ret;
        }

    }

}
