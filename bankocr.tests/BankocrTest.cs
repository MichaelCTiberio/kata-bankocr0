using BankOcr;
using System;
using Xunit;

namespace BankOcr.Tests
{
    public class DigitTests
    {
        [Theory]
        [InlineData('0')]
        [InlineData('1')]
        [InlineData('2')]
        [InlineData('3')]
        [InlineData('4')]
        [InlineData('5')]
        [InlineData('6')]
        [InlineData('7')]
        [InlineData('8')]
        [InlineData('9')]
        public void ShouldCreateDigits(char data)
        {
            char expected = data;

            Digit digit = Digit.MaybeFromChar(data).Value;
            char actual = digit.Value;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData('/')]
        [InlineData(':')]
        public void ShouldNotCreateNonDigits(char data)
        {
            // Expect null

            Digit? digit = Digit.MaybeFromChar(data);

            Assert.Null(digit);
        }

        [Theory]
        [InlineData("     |  |", '1')]
        public void ShouldConvert(string s, char expected)
        {
            Digit digit = Digit.MaybeFromString(s).Value;
            char actual = digit.Value;

            Assert.Equal(expected, actual);
        }
    }
}
