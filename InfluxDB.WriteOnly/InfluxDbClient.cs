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
        private readonly Uri endpoint;
        private readonly HttpClient httpClient = new HttpClient();

        public InfluxDbClient(Uri endpoint, string username = null, string password = null, TimeUnitPrecision precision = TimeUnitPrecision.Millisecond, bool throwOnException = false) {
            if (username != null && password == null || username == null && password != null) {
                throw new ArgumentException("When username or password is defined, both must be defined");
            }

            this.precision = precision;
            this.throwOnException = throwOnException;
            this.endpoint = new Uri(endpoint, "write");
            if (username != null) {
                var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            }
        }

        public async Task WriteAsync(string retentionPolicy, string dbName, IEnumerable<Point> points) {
            try {
                var uri = $"{endpoint}?{CreateQueryString(dbName, retentionPolicy)}";
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

        private static string CreateQueryString(string dbName, string retentionPolicy = null, TimeUnitPrecision precision = TimeUnitPrecision.Millisecond) {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            if (retentionPolicy != null) {
                queryString.Add("rp", retentionPolicy);
            }

            queryString.Add("precision", precision.ToPrecisionString());
            queryString.Add("db", dbName);
            return queryString.ToString();
        }
    }
}