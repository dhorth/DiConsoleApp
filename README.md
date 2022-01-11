# Dependency Injection for net5 console applications

An internet search will provide numerous results on how to use dependency injection in a dotnet console application.  My problem with a majority of the examples provided, including Microsoft own example, is that they wanted you to use the ServiceProvider to get the actual service you required.  See the example below
```
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ConsoleDI.Example
{
    class Program
    {
        static Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();

            ExemplifyScoping(host.Services, "Scope 1");
            ExemplifyScoping(host.Services, "Scope 2");

            return host.RunAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((_, services) =>
                    services.AddTransient<ITransientOperation, DefaultOperation>()
                            .AddScoped<IScopedOperation, DefaultOperation>()
                            .AddSingleton<ISingletonOperation, DefaultOperation>()
                            .AddTransient<OperationLogger>());

        static void ExemplifyScoping(IServiceProvider services, string scope)
        {
            using IServiceScope serviceScope = services.CreateScope();
            IServiceProvider provider = serviceScope.ServiceProvider;

            OperationLogger logger = provider.GetRequiredService<OperationLogger>();
            logger.LogOperations($"{scope}-Call 1 .GetRequiredService<OperationLogger>()");

            Console.WriteLine("...");

            logger = provider.GetRequiredService<OperationLogger>();
            logger.LogOperations($"{scope}-Call 2 .GetRequiredService<OperationLogger>()");

            Console.WriteLine();
        }
    }
}
```
The GetRequiredService works fine, but the code becomes a bit messy and difficult to read.  I wanted a simpler approach where the service where injected automatically.  More like this…
```
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

    /// <summary>
    /// This is where the magic is realized, TestClient is a simple implementation
    /// but notice the depency on IDummyService.  This is automatically injected
    /// in the ConsoleHelper.ConfigureService.  Furthermore, IDummyService depends
    /// on the AppSettings service, this is also injected.
    /// </summary>
    public class TestClient : ConsoleApplication
    {

        IDummyService _dummyService;
        public TestClient(IDummyService dummyService)
        {
            _dummyService = dummyService;
        }


        /// <summary>
        /// The business logic you want you Main to actually complete
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public override async Task<int> Run()
        {
            _dummyService.AmADummy();
            await _dummyService.NotADummyAsync();

            Log.Logger.Information("All done!  Press any key to exit");
            Console.ReadKey();
            return 0;
        }
    }
}
```


The code looks super clean and the end developer can focus on his/her business implementation without having to worry about a lot of boilerplate frame work.

Let’s take a look at the code to see how this was accomplished. The standard template for a console app is as follows
```
        static async Task<int> Main(string[] args)
        {
            var config = ConsoleHelper.InitializeConsole(AppName);
            var ret = await ConsoleHelper.ConfigureServices<TestClient>(config, (hostContext, service) =>
            {
                service.AddSingleton<IDummyService,DummyService>();
            });
            return ret;
        }
```


Using my ConsoleHelper class, I Initialize our base console application.  I like to have all my console apps support a configuration file, and use Serilog for logging purposes.
```
var config = ConsoleHelper.InitializeConsole(AppName);
```

Once that’s done I call my ConsoleHelper.ConfigureServices, this where the magic starts to unfold.  ConfigureServices needs an implenation of IConsoleApplication, IConfiguration, and then a delegate to any services my application requires.  In the case of this example the implementation is a class called TestClient, and the only service I require (besides the base services) is IDummyService.

```
            var ret = await ConsoleHelper.ConfigureServices<TestClient>(config, (hostContext, service) =>
            {
                service.AddSingleton<IDummyService,DummyService>();
            });
```
Inside ConfigureServices I am going to register all my services.  In this example I included AppSettings, I use this a lot in my application, but its just here to show the dependency injection.  The critical piece is the IConsoleApplication service.  By adding this service, I can now use the ServiceProvider to get it back and execute its Run method.
```
                service.AddSingleton<IDummyService,DummyService>();
```

Back at our sample application, I have created a TestClient class that inherits from a base ConsoleApplication.  This is not required, TestClient could have just implemented the IConsoleApplication interface directly but I like this pattern, it allows me to bury any core logic inside the ConsoleApplication.  Adjust to meet your needs!

```
    public class TestClient : ConsoleApplication
    {
        IDummyService _dummyService;
        public TestClient(IDummyService dummyService)
        {
            _dummyService = dummyService;
        }

        public override async Task<int> Run()
        {
            _dummyService.AmADummy();
            await _dummyService.NotADummyAsync();

            Log.Logger.Information("All done!  Press any key to exit");
            Console.ReadKey();
            return 0;
        }
    }
```
    
The construction for TestClient requires IDummyService which will now automatically get injected.  No need to use the ServiceProvider to get an instance.

```
    public interface IDummyService
    {
        Task NotADummyAsync();
        void AmADummy();
    }

    public class DummyService:IDummyService
    {
        private readonly AppSettings _appSettings;

        public DummyService(AppSettings appSettings)
        {
            _appSettings=appSettings;
        }

        public async Task NotADummyAsync()
        {
            await Task.Run(() =>
            {
                Log.Logger.Information($"Execute some async operation...");
            });
            Log.Logger.Information($"I am not a dummy");
        }

        public void AmADummy()
        {
            Log.Logger.Information($"Your Message: '{_appSettings.DummyMessage}'");
            Log.Logger.Warning($"My mistake, I am a dummy");
        }

    }

```
I also add AppSettings to the Constructor for DummeyService, this is just an example to show that the dependent services can be resolved.
```
        public DummyService(AppSettings appSettings)
        {
            _appSettings=appSettings;
        }
```

This is just an example, Please adjust to meet your needs

Full source code can be found on GitHub in [DiConsoleApp](https://github.com/dhorth/DiConsoleApp)
