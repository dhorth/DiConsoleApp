using Horth.Shared.Infrastructure;
using Serilog;
using System;
using System.Threading.Tasks;

namespace DiConsoleApp
{
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
