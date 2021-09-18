using Xunit;

namespace BankOcr.Domain.Test
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
            var digit = Digit.FromString(s);

            char actual = (char) digit;
            Assert.Equal(expected, actual);
        }
    }
}
