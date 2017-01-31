using System.Linq;
using System.Text;

namespace InfluxWriteOnly {
    public static class PointFormatter {
        public static void Append(StringBuilder sb, Point point) {
            sb.Append(point.Measurement);
            if (point.Tags.Any()) {
                sb.Append($",{string.Join(",", point.Tags)}");
            }

            sb.Append($" {string.Join(",", point.Fields)}");

            if (point.Timestamp.HasValue) {
                sb.AppendLine($" {point.Timestamp.Value.ToUnixTime()}");
            }
        }

        public static string Escape(string s) {
            var result = s
                .Replace(@"\", @"\\")
                .Replace(@"""", @"\""")
                .Replace(@" ", @"\ ")
                .Replace(@"=", @"\=")
                .Replace(@",", @"\,");

            return result;
        }
    }
}