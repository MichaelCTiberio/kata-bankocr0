using BankOcr;
using System;
using Xunit;

namespace BankOcr.Tests
{
    public class DigitTests
    {
        // [Theory]
        // [InlineData('0')]
        // [InlineData('1')]
        // [InlineData('2')]
        // [InlineData('3')]
        // [InlineData('4')]
        // [InlineData('5')]
        // [InlineData('6')]
        // [InlineData('7')]
        // [InlineData('8')]
        // [InlineData('9')]
        // public void ShouldCreateDigits(char data)
        // {
        //     char expected = data;

        //     Digit digit = Digit.MaybeFromChar(data).Value;
        //     char actual = digit.Value;

        //     Assert.Equal(expected, actual);
        // }

        // [Theory]
        // [InlineData('/')]
        // [InlineData(':')]
        // public void ShouldNotCreateNonDigits(char data)
        // {
        //     // Expect null

        //     Digit? digit = Digit.MaybeFromChar(data);

        //     Assert.Null(digit);
        // }

        [Theory]
        [InlineData(DigitStrings.Zero, '0')]
        [InlineData(DigitStrings.One, '1')]
        [InlineData(DigitStrings.Two, '2')]
        [InlineData(DigitStrings.Three, '3')]
        [InlineData(DigitStrings.Four, '4')]
        [InlineData(DigitStrings.Five, '5')]
        [InlineData(DigitStrings.Six, '6')]
        [InlineData(DigitStrings.Seven, '7')]
        [InlineData(DigitStrings.Eight, '8')]
        [InlineData(DigitStrings.Nine, '9')]
        public void ShouldConvert(string s, char expected)
        {
            var maybeDigit = Digit.MaybeFromString(s);

            Assert.True(maybeDigit.Valid);
            char actual = maybeDigit.Result.Value;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ShouldNotConvert()
        {
            string s = "bad string";
            string expected = $"Invalid string \"{s}\"";

            var maybeDigit = Digit.MaybeFromString(s);

            Assert.False(maybeDigit.Valid);
            string actual = maybeDigit.Error;

            Assert.Equal(expected, actual);
        }
    }
}
