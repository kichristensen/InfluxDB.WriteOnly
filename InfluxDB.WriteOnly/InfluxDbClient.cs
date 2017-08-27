using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace InfluxDB.WriteOnly
{
    public interface IInfluxDbClient
    {
        Task WriteAsync(string retentionPolicy, string dbName, IEnumerable<Point> points);

        Task WriteAsync(string dbName, IEnumerable<Point> points);
    }

    public class InfluxDbClient : IInfluxDbClient
    {
        private readonly InfluxDbClientOptions options;
        private readonly UriBuilder endpoint;

        public InfluxDbClient(Uri endpoint, InfluxDbClientOptions options)
        {
            this.options = options;
            this.endpoint = new UriBuilder(new Uri(endpoint, "write")) {Query = endpoint.Query.TrimStart('?')};
        }

        public async Task WriteAsync(string retentionPolicy, string dbName, IEnumerable<Point> points)
        {
            try
            {
                var uri = CreateQueryString(endpoint, options.Login, dbName, retentionPolicy).Uri;
                options.Logger.Debug($"Using uri: {uri}");
                var formatPoints = points.FormatPoints(options.Precision);
                options.Logger.Trace($"Formatted points:\n{points}");
                options.Logger.Debug("Sending request...");
                var request = WebRequest.CreateHttp(uri);
                options.RequestConfigurator(request);
                request.Method = "POST";
                using (var stream = new StreamWriter(request.GetRequestStream()))
                {
                    stream.Write(formatPoints);
                }
                using (var response = (HttpWebResponse) await request.GetResponseAsync().ConfigureAwait(false))
                {
                    if (response.StatusCode != HttpStatusCode.NoContent)
                    {
                        options.Logger.Error($"Got response with error code {response.StatusCode}");
                        string content;
                        using (var stream = new StreamReader(response.GetResponseStream()))
                        {
                            content = await stream.ReadToEndAsync().ConfigureAwait(false);
                        }
                        throw new HttpRequestException(
                            $"Got status code {response.StatusCode} with content:\r\n{content}");
                    }

                    options.Logger.Debug("Request sent");
                }
            }
            catch (Exception e) when (!options.ThrowOnExceptions)
            {
                options.Logger.Error($"Got exception while sending request to InfluxDB:\n{e}");
            }
        }

        public async Task WriteAsync(string dbName, IEnumerable<Point> points)
        {
            await WriteAsync(null, dbName, points).ConfigureAwait(false);
        }

        private static UriBuilder CreateQueryString(UriBuilder endpoint, LoginInformation login,
            string dbName, string retentionPolicy = null, TimeUnitPrecision precision = TimeUnitPrecision.Millisecond)
        {
            var updatedEndpoint = new UriBuilder(endpoint.Uri);
            var queryString = HttpUtility.ParseQueryString(updatedEndpoint.Query);
            if (retentionPolicy != null)
            {
                queryString.Add("rp", retentionPolicy);
            }

            queryString.Add("precision", precision.ToPrecisionString());
            queryString.Add("db", dbName);
            if (login != null)
            {
                queryString.Add("u", login.Username);
                queryString.Add("p", login.Password);
            }
            updatedEndpoint.Query = queryString.ToString();
            return updatedEndpoint;
        }
    }
}