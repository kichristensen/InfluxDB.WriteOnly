using System;

namespace InfluxDB.WriteOnly
{
    public static class DateTimeExtensions
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static long ToUnixTime(this DateTime dateTime,
            TimeUnitPrecision precision = TimeUnitPrecision.Millisecond)
        {
            var span = dateTime - Epoch;
            double fractionalSpan;
            switch (precision)
            {
                case TimeUnitPrecision.Millisecond:
                    fractionalSpan = span.TotalMilliseconds;
                    break;
                case TimeUnitPrecision.Second:
                    fractionalSpan = span.TotalSeconds;
                    break;
                case TimeUnitPrecision.Minute:
                    fractionalSpan = span.TotalMinutes;
                    break;
                case TimeUnitPrecision.Hour:
                    fractionalSpan = span.TotalHours;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(precision), precision, null);
            }

            return Convert.ToInt64(fractionalSpan);
        }
    }
}