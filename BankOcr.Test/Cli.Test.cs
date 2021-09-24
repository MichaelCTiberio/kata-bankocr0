using BankOcr.Domain;
using FunLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace BankOcr.Cli.Tests
{
    public class FileReaderTests
    {
        [Fact]
        public void ShouldEnumerateLines()
        {
            List<string> expected = new ()
            {
                "This is a bunch",
                "Of multi line text",
                "That we can use",
                "In a test.",
            };

            string text = string.Join("\n", expected);

            StringReader reader = new (text);
            IEnumerable<string> actual = FileReader.Lines(reader).Value;

            Assert.Equal(expected, actual);
        }
    }

    public class ProgramTests
    {
        [Fact]
        public void FilenameFromArgsShouldSucceed()
        {
            string expected = "file path";
            Func<IndexOutOfRangeException, bool> handler =
                Fn.Handler<IndexOutOfRangeException>(
                    (ex) => throw new InvalidOperationException("Could not find file name."));

            var hasFilename = Program.FilenameFromArgs(new [] { expected }, handler);

            Assert.True(hasFilename.HasValue());
            string actual = hasFilename.Value;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void FilenameFromArgsShouldFail()
        {
            Func<IndexOutOfRangeException, bool> noHandler = (ex) => false;

            Assert.Throws<IndexOutOfRangeException>(() => Program.FilenameFromArgs(new string[] { }, noHandler));
        }

        [Theory]
        [InlineData("000000000")]
        [InlineData("111111111")]
        [InlineData("222222222")]
        [InlineData("333333333")]
        [InlineData("444444444")]
        [InlineData("555555555")]
        [InlineData("666666666")]
        [InlineData("777777777")]
        [InlineData("888888888")]
        [InlineData("999999999")]
        [InlineData("123456789")]
        public void ShouldGetAccountNumbersFromTextStream(string expected)
        {
            var lines = TestLib.AccountLinesFromAccountNumber(expected);
            var accounts = Program.AccountNumbersFromTextLines(lines);

            Assert.Single(accounts);
            string actual = accounts.First().Number;
            Assert.Equal(expected, actual);
        }
    }

    public static class TestLib
    {
        public static IEnumerable<string> AccountLinesFromAccountNumber(string accountNumber) =>
            accountNumber
                .ToDigits()
                .TextLines();

        private static IEnumerable<DigitBuilder> ToDigits(this string accountNumber) =>
            accountNumber
                .AsEnumerable()
                .Select(DigitBuilder.FromChar);

        private static IEnumerable<string> TextLines(this IEnumerable<DigitBuilder> digits)
        {
            const char Delimiter = ' ';

            yield return digits.ConcatDigits(DigitBuilder.Top, Delimiter);
            yield return digits.ConcatDigits(DigitBuilder.Middle, Delimiter);
            yield return digits.ConcatDigits(DigitBuilder.Bottom, Delimiter);
            yield return "";
        }

        private static string ConcatDigits(this IEnumerable<DigitBuilder> digit, Func<DigitBuilder, string> func, char delimiter) =>
            digit
                .Select(func)
                .Concat(delimiter);

        private static string Concat(this IEnumerable<string> strings, char delimiter) =>
            (new StringBuilder())
                .AppendJoin(delimiter, strings)
                .ToString();
    }
}
