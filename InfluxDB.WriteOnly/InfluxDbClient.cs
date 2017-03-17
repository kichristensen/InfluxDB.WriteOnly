using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace InfluxDB.WriteOnly {
    public interface IInfluxDbClient {
        Task WriteAsync(string retentionPolicy, string dbName, IEnumerable<Point> points);

        Task WriteAsync(string dbName, IEnumerable<Point> points);
    }

    public class InfluxDbClient : IInfluxDbClient {
        private readonly string username;
        private readonly string password;
        private readonly TimeUnitPrecision precision;
        private readonly bool throwOnException;
        private readonly UriBuilder endpoint;

        public Action<HttpWebRequest> RequestConfigurator { get; set; } = request => { };

        public InfluxDbClient(Uri endpoint, string username = null, string password = null, TimeUnitPrecision precision = TimeUnitPrecision.Millisecond, bool throwOnException = false) {
            if (username != null && password == null || username == null && password != null) {
                throw new ArgumentException("When username or password is defined, both must be defined");
            }

            this.username = username;
            this.password = password;
            this.precision = precision;
            this.throwOnException = throwOnException;
            this.endpoint = new UriBuilder(new Uri(endpoint, "write")) { Query = endpoint.Query.TrimStart('?') };
        }

        public async Task WriteAsync(string retentionPolicy, string dbName, IEnumerable<Point> points) {
            try {
                var uri = CreateQueryString(endpoint, username, password, dbName, retentionPolicy).Uri;
                var formatPoints = points.FormatPoints(precision);
                var request = WebRequest.CreateHttp(uri);
                RequestConfigurator(request);
                request.Method = "POST";
                using (var stream = new StreamWriter(request.GetRequestStream())) {
                    stream.Write(formatPoints);
                }
                using (var response = (HttpWebResponse)await request.GetResponseAsync()) {
                    if (response.StatusCode != HttpStatusCode.NoContent) {
                        string content;
                        using (var stream = new StreamReader(response.GetResponseStream())) {
                            content = await stream.ReadToEndAsync();
                        }
                        throw new HttpRequestException($"Got status code {response.StatusCode} with content:\r\n{content}");
                    }
                }
            } catch (Exception e) when (!throwOnException) { 
                Debug.WriteLine("Exception occured while written to InfluxDB:\n{0}", e);
            }
        }

        public async Task WriteAsync(string dbName, IEnumerable<Point> points) {
            await WriteAsync(null, dbName, points);
        }

        private static UriBuilder CreateQueryString(UriBuilder endpoint, string username, string password, string dbName, string retentionPolicy = null, TimeUnitPrecision precision = TimeUnitPrecision.Millisecond) {
            var updatedEndpoint = new UriBuilder(endpoint.Uri);
            var queryString = HttpUtility.ParseQueryString(updatedEndpoint.Query);
            if (retentionPolicy != null) {
                queryString.Add("rp", retentionPolicy);
            }

            queryString.Add("precision", precision.ToPrecisionString());
            queryString.Add("db", dbName);
            queryString.Add("u", username);
            queryString.Add("p", password);
            updatedEndpoint.Query = queryString.ToString();
            return updatedEndpoint;
        }
    }
}