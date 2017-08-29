using System;
using FluentAssertions;
using Xunit;

namespace InfluxDB.WriteOnly.Tests
{
    public class LoginInformationTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void WhitespaceOrNullUsernameIsNotAllowed(string username)
        {
            Action action = () => new LoginInformation(username, "anything");

            action.ShouldThrow<ArgumentException>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void WhitespaceOrNullPasswordIsNotAllowed(string password)
        {
            Action action = () => new LoginInformation("anything", password);

            action.ShouldThrow<ArgumentException>();
        }
    }
}