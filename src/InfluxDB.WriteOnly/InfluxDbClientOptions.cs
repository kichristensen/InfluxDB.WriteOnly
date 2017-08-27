using System;
using System.Net;

namespace InfluxDB.WriteOnly
{
    public class InfluxDbClientOptions
    {
        public LoginInformation Login { get; set; }
        public TimeUnitPrecision Precision { get; set; } = TimeUnitPrecision.Millisecond;
        public bool ThrowOnExceptions { get; set; } = false;
        public Action<HttpWebRequest> RequestConfigurator { get; set; } = _ => { };
        public ILogger Logger { get; set; } = new DefaultLogger();
    }
}