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
            Maybe<Digit> maybeDigit = Digit.FromString(s);
            Assert.True(maybeDigit);

            char actual = (Digit) maybeDigit;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ShouldNotConvert()
        {
            string s = "bad string";

            Maybe<Digit> maybeDigit = Digit.FromString(s);
            Assert.False(maybeDigit);
        }
    }
}
