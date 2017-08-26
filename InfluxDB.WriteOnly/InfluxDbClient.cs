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
        private readonly InfluxDbClientOptions options;
        private readonly UriBuilder endpoint;

        public InfluxDbClient(Uri endpoint, InfluxDbClientOptions options) {
            if (options.Username != null && options.Password == null || options.Username == null && options.Password != null) {
                throw new ArgumentException("When username or password is defined, both must be defined");
            }

            this.options = options;
            this.endpoint = new UriBuilder(new Uri(endpoint, "write")) { Query = endpoint.Query.TrimStart('?') };
        }

        public async Task WriteAsync(string retentionPolicy, string dbName, IEnumerable<Point> points) {
            try {
                var uri = CreateQueryString(endpoint, options.Username, options.Password, dbName, retentionPolicy).Uri;
                var formatPoints = points.FormatPoints(options.Precision);
                var request = WebRequest.CreateHttp(uri);
                options.RequestConfigurator(request);
                request.Method = "POST";
                using (var stream = new StreamWriter(request.GetRequestStream())) {
                    stream.Write(formatPoints);
                }
                using (var response = (HttpWebResponse)await request.GetResponseAsync().ConfigureAwait(false)) {
                    if (response.StatusCode != HttpStatusCode.NoContent) {
                        string content;
                        using (var stream = new StreamReader(response.GetResponseStream())) {
                            content = await stream.ReadToEndAsync().ConfigureAwait(false);
                        }
                        throw new HttpRequestException($"Got status code {response.StatusCode} with content:\r\n{content}");
                    }
                }
            }
            catch (Exception e) when (!options.ThrowOnExceptions)
            {
                options.ExceptionHandler(e);
            }
        }

        public async Task WriteAsync(string dbName, IEnumerable<Point> points) {
            await WriteAsync(null, dbName, points).ConfigureAwait(false);
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