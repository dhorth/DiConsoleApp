using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horth.Shared.Infrastructure
{
    /// <summary>
    /// Simple interace for the Console Appliocation
    /// </summary>
    public interface IConsoleApplication
    {
        Task<int> Run();
    }

    /// <summary>
    /// Not truely required, but its nice to be able to do some base class 
    /// work here, that way the higher level implemenation can focus on the 
    /// business logic
    /// </summary>
    public abstract class ConsoleApplication : IConsoleApplication
    {
        protected ConsoleApplication()
        {
            Log.Logger.Debug("ConsoleApplication created");
        }

        /// <summary>
        /// Execute the business logic in this override
        /// </summary>
        /// <returns></returns>
        public abstract Task<int> Run();


    }
}
