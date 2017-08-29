using FluentAssertions;
using Xunit;

namespace InfluxDB.WriteOnly.Tests
{
    public class TagTests
    {
        [Fact]
        public void StringRepresentationIsCorrect()
        {
            var tag = new Tag("key", "value");

            var result = tag.ToString();

            result.Should().Be($"{tag.Key}={tag.Value}");
        }

        [Fact]
        public void TagsAreEqualIfKeyAndValueAreEqual()
        {
            var tag1 = new Tag("test", "testValue");
            var tag2 = new Tag("test", "testValue");

            var isEqual = tag1.Equals(tag2);

            isEqual.Should().BeTrue();
        }
    }
}