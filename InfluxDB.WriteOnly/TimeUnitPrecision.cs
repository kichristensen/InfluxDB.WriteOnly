using System;

namespace InfluxDB.WriteOnly {
    public enum TimeUnitPrecision {
        Millisecond,
        Second,
        Minute,
        Hour
    }

    public static class TimeUnitPrecisionExtentions {
        public static string ToPrecisionString(this TimeUnitPrecision precision) {
            switch (precision) {
                case TimeUnitPrecision.Millisecond:
                    return "ms";
                case TimeUnitPrecision.Second:
                    return "s";
                case TimeUnitPrecision.Minute:
                    return "m";
                case TimeUnitPrecision.Hour:
                    return "h";
                default:
                    throw new ArgumentOutOfRangeException(nameof(precision), precision, null);
            }
        }
    }
}