using System.Linq;
using System.Text;
using FluentAssertions;
using Xunit;

namespace InfluxDB.WriteOnly.Tests
{
    public class PointFormatterTests
    {
        [Theory]
        [InlineData(@"\", @"\\")]
        [InlineData(@"""", @"\""")]
        [InlineData(@" ", @"\ ")]
        [InlineData(@"=", @"\=")]
        [InlineData(@",", @"\,")]
        public void EscapingAreHandledAccordingToLineProtocol(string input, string expectedOutput)
        {
            PointFormatter.Escape(input).Should().Be(expectedOutput);
        }

        [Fact]
        public void PointsAreFormatterAccordingToLineProtocol()
        {
            var sb = new StringBuilder();
            var point = new Point
            {
                Measurement = "measurement",
                Fields = new[] { new Field("fieldKey", "fieldValue"), new Field("count", 1) },
                Tags = new [] { new Tag("tagKey", "tagValue"), new Tag("Key2", "value2") },
                Timestamp = 1.January(1970)
            };

            PointFormatter.Append(sb, point, TimeUnitPrecision.Millisecond);

            var result = sb.ToString();
            result.Should()
                .Be(
                    $"{point.Measurement},{string.Join(",", point.Tags)} {point.Fields.First().Key}=\"{point.Fields.First().Value}\",{point.Fields.Last().Key}={point.Fields.Last().Value}i 0\n");
        }
    }
}