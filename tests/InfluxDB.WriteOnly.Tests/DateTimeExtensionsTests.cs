using System;
using FluentAssertions;
using InfluxDB.WriteOnly.Tests.Helpers;
using Xunit;

namespace InfluxDB.WriteOnly.Tests
{
    public class DateTimeExtensionsTests
    {
        [Theory]
        [EnumTheory(typeof(TimeUnitPrecision))]
        public void UnixEpochGivesUnixTimestampOfZero(TimeUnitPrecision precision)
        {
            var unixEpoch = 1.January(1970);

            var timestamp = unixEpoch.ToUnixTime(precision);

            timestamp.Should().Be(0);
        }

        [Theory]
        [InlineData(TimeUnitPrecision.Millisecond, 1501545600000)]
        [InlineData(TimeUnitPrecision.Second, 1501545600)]
        [InlineData(TimeUnitPrecision.Minute, 25025760)]
        [InlineData(TimeUnitPrecision.Hour, 417096)]
        public void FirstOfAugust2017NoonGivesCorrectUnixTimestamp(TimeUnitPrecision precision, long expectedTimestamp)
        {
            var dateTime = 1.August(2017);

            var timestamp = dateTime.ToUnixTime(precision);

            timestamp.Should().Be(expectedTimestamp);
        }
    }
}
