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

            Digit digit = Digit.MaybeNew(data).Value;
            char actual = digit.Value;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData('/')]
        [InlineData(':')]
        public void ShouldNotCreateNonDigits(char data)
        {
            // Expect null

            Digit? digit = Digit.MaybeNew(data);

            Assert.Null(digit);
        }
    }
}
