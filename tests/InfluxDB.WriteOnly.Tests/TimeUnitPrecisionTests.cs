using FluentAssertions;
using Xunit;

namespace InfluxDB.WriteOnly.Tests
{
    public class TimeUnitPrecisionTests
    {
        [Theory]
        [InlineData(TimeUnitPrecision.Millisecond, "ms")]
        [InlineData(TimeUnitPrecision.Second, "s")]
        [InlineData(TimeUnitPrecision.Minute, "m")]
        [InlineData(TimeUnitPrecision.Hour, "h")]
        public void StringRepresentationFollowsLineProtocolSpecification(TimeUnitPrecision precision, string expected)
        {
            precision.ToPrecisionString().Should().Be(expected);
        }
    }
}