using System.Collections.Generic;
using System.Text;

namespace InfluxDB.WriteOnly {
    public static class PointExtensions {
        public static string FormatPoints(this IEnumerable<Point> points, TimeUnitPrecision precision = TimeUnitPrecision.Millisecond) {
            var sb = new StringBuilder();
            foreach (var point in points) {
                PointFormatter.Append(sb, point, precision);
            }

            return sb.ToString();
        }
    }
}