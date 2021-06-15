using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Horth.Service.Email.Shared.Configuration
{
    /// <summary>
    /// I use this class a lot in my applications
    /// I just included it here to use as an example
    /// of the second level injection
    /// </summary>
    public class AppSettings
    {
        private readonly IConfiguration _configuration;
        private static readonly object _lock = new object();


        public AppSettings(IConfiguration c)
        {
            _configuration = c;
        }

        public string DummyMessage => GetValue("DummyMessage", $"defaultValue");



        protected string GetValue(string name, string defaultValue = "")
        {
            return _configuration.GetValue(name, defaultValue);
        }
        protected T GetValue<T>(string name, T defaultValue)
        {
            return _configuration.GetValue(name, defaultValue);
        }
        protected T GetEnumValue<T>(string name, string defaultValue)
        {
            var strValue= _configuration.GetValue(name, defaultValue);
            var ret=(T)Enum.Parse(typeof(T), strValue);
            return ret;
        }

    }

}
