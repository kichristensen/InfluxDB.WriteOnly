using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace InfluxDB.WriteOnly {
    public interface IInfluxDbClient {
        Task WriteAsync(string retentionPolicy, string dbName, IEnumerable<Point> points);

        Task WriteAsync(string dbName, IEnumerable<Point> points);
    }

    public class InfluxDbClient : IInfluxDbClient {
        private readonly TimeUnitPrecision precision;
        private readonly bool throwOnException;
        private readonly UriBuilder endpoint;
        private readonly HttpClient httpClient = new HttpClient();

        public InfluxDbClient(Uri endpoint, string username = null, string password = null, TimeUnitPrecision precision = TimeUnitPrecision.Millisecond, bool throwOnException = false) {
            if (username != null && password == null || username == null && password != null) {
                throw new ArgumentException("When username or password is defined, both must be defined");
            }

            this.precision = precision;
            this.throwOnException = throwOnException;
            this.endpoint = new UriBuilder(new Uri(endpoint, "write")) { Query = endpoint.Query.TrimStart('?') };
            if (username != null) {
                var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            }
        }

        public async Task WriteAsync(string retentionPolicy, string dbName, IEnumerable<Point> points) {
            try {
                var uri = CreateQueryString(endpoint, dbName, retentionPolicy).Uri;
                var formatPoints = points.FormatPoints(precision);
                var response = await httpClient.PostAsync(uri, new StringContent(formatPoints));
                response.EnsureSuccessStatusCode();
            } catch (Exception e) when (!throwOnException) {
                Debug.WriteLine("Exception occured while written to InfluxDB:\n{0}", e);
            }
        }

        public async Task WriteAsync(string dbName, IEnumerable<Point> points) {
            await WriteAsync(null, dbName, points);
        }

        private static UriBuilder CreateQueryString(UriBuilder endpoint, string dbName, string retentionPolicy = null, TimeUnitPrecision precision = TimeUnitPrecision.Millisecond) {
            var queryString = HttpUtility.ParseQueryString(endpoint.Query);
            if (retentionPolicy != null) {
                queryString.Add("rp", retentionPolicy);
            }

            queryString.Add("precision", precision.ToPrecisionString());
            queryString.Add("db", dbName);
            endpoint.Query = queryString.ToString();
            return endpoint;
        }
    }
}