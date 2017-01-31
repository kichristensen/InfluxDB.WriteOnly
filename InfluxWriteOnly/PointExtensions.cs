using System.Collections.Generic;
using System.Text;

namespace InfluxWriteOnly {
    public static class PointExtensions {
        public static string FormatPoints(this IEnumerable<Point> points) {
            var sb = new StringBuilder();
            foreach (var point in points) {
                PointFormatter.Append(sb, point);
            }

            return sb.ToString();
        }
    }
}