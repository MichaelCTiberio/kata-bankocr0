using BankOcr;
using System;
using Xunit;

namespace BankOcr.Tests
{
    public class DigitTests
    {
        [Theory]
        [InlineData(Digit.Zero, '0')]
        [InlineData(Digit.One, '1')]
        [InlineData(Digit.Two, '2')]
        [InlineData(Digit.Three, '3')]
        [InlineData(Digit.Four, '4')]
        [InlineData(Digit.Five, '5')]
        [InlineData(Digit.Six, '6')]
        [InlineData(Digit.Seven, '7')]
        [InlineData(Digit.Eight, '8')]
        [InlineData(Digit.Nine, '9')]
        public void ShouldConvert(string s, char expected)
        {
            Result<Digit> maybeDigit = Digit.MaybeFromString(s);
            Assert.True(maybeDigit.Valid);

            char actual = maybeDigit.Value.Value;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ShouldNotConvert()
        {
            string s = "bad string";
            string expected = $"Invalid string \"{s}\"";

            Result<Digit> maybeDigit = Digit.MaybeFromString(s);
            Assert.False(maybeDigit.Valid);

            string actual = maybeDigit.Error;
            Assert.Equal(expected, actual);
        }
    }
}
