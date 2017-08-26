using System;
using System.Diagnostics;
using System.Net;

namespace InfluxDB.WriteOnly
{
    public class InfluxDbClientOptions
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public TimeUnitPrecision Precision { get; set; } = TimeUnitPrecision.Millisecond;
        public bool ThrowOnExceptions { get; set; } = false;
        public Action<HttpWebRequest> RequestConfigurator { get; set; } = _ => { };
        public Action<Exception> ExceptionHandler { get; set; } = e => Debug.WriteLine("Exception occured while written to InfluxDB:\n{0}", e);
    }
}