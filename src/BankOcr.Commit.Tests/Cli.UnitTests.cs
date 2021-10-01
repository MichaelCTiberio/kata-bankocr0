using BankOcr.Cli;
using BankOcr.Domain;
using FunLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace BankOcr.Tests.Unit.Cli
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
            string expected = @"c:\path\to\some\file";
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
        [InlineData("987654321")]
        public void ShouldGetAccountNumbersFromTextStream(string expected)
        {
            var lines = TestLib.AccountLinesFromAccountNumber(expected);
            var accounts = Program.AccountNumbersFromTextLines(lines);

            Assert.Single(accounts);
            string actual = accounts.First().Number();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ShouldHandleNoAccountNumbers()
        {
            var lines = EmptyEnumerable();
            var accounts = Program.AccountNumbersFromTextLines(lines);

            Assert.Empty(accounts);

            static IEnumerable<string> EmptyEnumerable()
            {
                yield break;
            }
        }

        [Fact]
        public void Shouldhandle1000AccountNumbers()
        {
            var expected = TestLib.GenerateAccountNumbers(1000);
            var lines = TestLib.AccountLinesForAccountNumbers(expected);
            var accounts = Program.AccountNumbersFromTextLines(lines);

            var actual = accounts.Select(AccountHelpers.Number);
            Assert.Equal(expected, actual);
        }
    }

    public static class TestLib
    {
        public static IEnumerable<string> GenerateAccountNumbers(int count)
        {
            for (int n = 0; n < count; n++)
                yield return (n * 1000).ToString("000000000");
        }

        public static IEnumerable<string> AccountLinesForAccountNumbers(IEnumerable<string> accountNumbers)
        {
            foreach (var accountNumber in accountNumbers)
                foreach (var line in AccountLinesFromAccountNumber(accountNumber))
                    yield return line;
        }

        public static IEnumerable<string> AccountLinesFromAccountNumber(string accountNumber) =>
            accountNumber
                .TextLines();

        private static IEnumerable<string> TextLines(this string digits)
        {
            yield return digits.ConcatDigits(DigitBuilder.Top);
            yield return digits.ConcatDigits(DigitBuilder.Middle);
            yield return digits.ConcatDigits(DigitBuilder.Bottom);
            yield return "";
        }

        private static string ConcatDigits(this string digit, Func<char, string> func) =>
            digit
                .Select(func)
                .Concat();

        private static string Concat(this IEnumerable<string> strings) =>
            (new StringBuilder())
                .AppendJoin("", strings)
                .ToString();

        private static class DigitBuilder
        {
            private const string ZeroTop = " _ ";
            private const string ZeroMid = "| |";
            private const string ZeroBot = "|_|";

            private const string OneTop = "   ";
            private const string OneMid = "  |";
            private const string OneBot = "  |";

            private const string TwoTop = " _ ";
            private const string TwoMid = " _|";
            private const string TwoBot = "|_ ";

            private const string ThreeTop = " _ ";
            private const string ThreeMid = " _|";
            private const string ThreeBot = " _|";

            private const string FourTop = "   ";
            private const string FourMid = "|_|";
            private const string FourBot = "  |";

            private const string FiveTop = " _ ";
            private const string FiveMid = "|_ ";
            private const string FiveBot = " _|";

            private const string SixTop = " _ ";
            private const string SixMid = "|_ ";
            private const string SixBot = "|_|";

            private const string SevenTop = " _ ";
            private const string SevenMid = "  |";
            private const string SevenBot = "  |";

            private const string EightTop = " _ ";
            private const string EightMid = "|_|";
            private const string EightBot = "|_|";

            private const string NineTop = " _ ";
            private const string NineMid = "|_|";
            private const string NineBot = " _|";


            public static string Top(char d) =>
                d switch
                {
                    '0' => ZeroTop,
                    '1' => OneTop,
                    '2' => TwoTop,
                    '3' => ThreeTop,
                    '4' => FourTop,
                    '5' => FiveTop,
                    '6' => SixTop,
                    '7' => SevenTop,
                    '8' => EightTop,
                    '9' => NineTop,
                    _ => throw new ArgumentException("Invalid input", nameof(d))
                };

            public static string Middle(char d) =>
                d switch
                {
                    '0' => ZeroMid,
                    '1' => OneMid,
                    '2' => TwoMid,
                    '3' => ThreeMid,
                    '4' => FourMid,
                    '5' => FiveMid,
                    '6' => SixMid,
                    '7' => SevenMid,
                    '8' => EightMid,
                    '9' => NineMid,
                    _ => throw new ArgumentException("Invalid input", nameof(d))
                };

            public static string Bottom(char d) =>
                d switch
                {
                    '0' => ZeroBot,
                    '1' => OneBot,
                    '2' => TwoBot,
                    '3' => ThreeBot,
                    '4' => FourBot,
                    '5' => FiveBot,
                    '6' => SixBot,
                    '7' => SevenBot,
                    '8' => EightBot,
                    '9' => NineBot,
                    _ => throw new ArgumentException("Invalid input", nameof(d))
                };
        }
    }
}
