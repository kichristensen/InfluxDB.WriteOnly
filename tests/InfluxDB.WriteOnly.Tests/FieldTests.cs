using System.Globalization;
using FluentAssertions;
using Xunit;

namespace InfluxDB.WriteOnly.Tests
{
    public class FieldTests
    {
        [Fact]
        public void AFloatFieldsStringRepresentationFollowsLineProtocolSpecification()
        {
            const float value = 10.4f;
            const string key = "float";
            var field = new Field(key, value);

            var s = field.ToString();

            s.Should().Be($"{key}={value.ToString(CultureInfo.InvariantCulture)}");
        }

        [Fact]
        public void AIntegerFieldsStringRepresentationFollowsLineProtocolSpecification()
        {
            const int value = 67;
            const string key = "int";
            var field = new Field(key, value);

            var s = field.ToString();

            s.Should().Be($"{key}={value}i");
        }

        [Fact]
        public void ALongFieldsStringRepresentationFollowsLineProtocolSpecification()
        {
            const long value = 123;
            const string key = "long";
            var field = new Field(key, value);

            var s = field.ToString();

            s.Should().Be($"{key}={value}i");
        }

        [Fact]
        public void AStringFieldsStringRepresentationFollowsLineProtocolSpecification()
        {
            const string value = "String value";
            const string expectedValue = "String\\ value";
            const string key = "string";
            var field = new Field(key, value);

            var s = field.ToString();

            s.Should().Be($"{key}=\"{expectedValue}\"");
        }

        [Fact]
        public void ATrueBooleanFieldsStringRepresentationFollowsLineProtocolSpecification()
        {
            const bool value = true;
            const string key = "bool";
            var field = new Field(key, value);

            var s = field.ToString();

            s.Should().Be($"{key}=t");
        }

        [Fact]
        public void AFalseBooleanFieldsStringRepresentationFollowsLineProtocolSpecification()
        {
            const bool value = false;
            const string key = "bool";
            var field = new Field(key, value);

            var s = field.ToString();

            s.Should().Be($"{key}=f");
        }

        [Fact]
        public void FielsAreEqualIfTheyHaveSameKeyAndValue()
        {
            var field1 = new Field("test", "value");
            var field2 = new Field("test", "value");

            var isEqual = field1.Equals(field2);

            isEqual.Should().BeTrue();
        }
    }
}