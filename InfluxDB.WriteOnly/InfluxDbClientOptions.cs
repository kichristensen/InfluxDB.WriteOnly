using System;
using System.Net;

namespace InfluxDB.WriteOnly
{
    public class InfluxDbClientOptions
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public TimeUnitPrecision Precision { get; set; } = TimeUnitPrecision.Millisecond;
        public bool ThrowOnExceptions { get; set; } = false;
        public Action<HttpWebRequest> RequestConfigurator { get; set; } = request => { };
    }
}