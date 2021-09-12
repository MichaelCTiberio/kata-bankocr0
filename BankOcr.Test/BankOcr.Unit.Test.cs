using System;
using System.Collections.Generic;
using System.IO;
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
            Maybe<Digit> hasDigit = Digit.FromString(s);
            Assert.True(hasDigit);

            char actual = hasDigit.Value;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ShouldNotConvert()
        {
            string s = "bad string";

            Maybe<Digit> noDigit = Digit.FromString(s);
            Assert.False(noDigit.HasValue);
        }
    }

    public class FileReaderTests
    {
        [Fact]
        public void ShouldEnumerateLines()
        {
            var expected = new List<string>
            {
                "This is a bunch",
                "Of multi line text",
                "That we can use",
                "In a test.",
            };

            string text = string.Join("\n", expected);

            StringReader reader = new StringReader(text);
            IEnumerable<string> actual = FileReader.Lines(reader).Value;

            Assert.Equal(expected, actual);
        }
    }

    public class UtilityTests
    {
        [Fact]
        public void TypeIs()
        {
            Exception ex = new NullReferenceException();

            var hasEx = ex.MaybeIs<NullReferenceException>();

            Assert.True(hasEx.HasValue);
        }

        [Fact]
        public void TypeIsNot()
        {
            Exception ex = new NullReferenceException();

            var noEx = ex.MaybeIs<string>();

            Assert.False(noEx.HasValue);
        }
    }
}
