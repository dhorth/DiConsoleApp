using Horth.Service.Email.Shared.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiConsoleApp
{
    /// <summary>
    /// A sample interface for our service, replace with your own implementation
    /// </summary>
    public interface IDummyService
    {
        Task NotADummyAsync();
        void AmADummy();
    }

    /// <summary>
    /// Implementation of the IDummyService class
    /// </summary>
    public class DummyService:IDummyService
    {
        private readonly AppSettings _appSettings;

        /// <summary>
        /// I wanted an example of depenency injection in our service
        /// so I am injecting my AppSettings service here, you can inject
        /// any registered service
        /// </summary>
        /// <param name="appSettings"></param>
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
}
