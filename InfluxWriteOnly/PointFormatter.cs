using System.Linq;
using System.Text;

namespace InfluxWriteOnly {
    public static class PointFormatter {
        public static void Append(StringBuilder sb, Point point, TimeUnitPrecision precision = TimeUnitPrecision.Millisecond) {
            sb.Append(Escape(point.Measurement));
            if (point.Tags.Any()) {
                sb.Append($",{string.Join(",", point.Tags)}");
            }

            sb.Append($" {string.Join(",", point.Fields)}");

            if (point.Timestamp.HasValue) {
                sb.Append($" {point.Timestamp.Value.ToUnixTime(precision)}\n");
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