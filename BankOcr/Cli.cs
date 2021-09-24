using BankOcr.Domain;
using FunLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BankOcr.Cli
{
    public static class FileReader
    {
        private static IEnumerable<string> LinesEnumerable(TextReader reader)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
                yield return line;
        }

        public static Maybe<IEnumerable<string>> Lines(TextReader reader) =>
            Fn.Try(() => LinesEnumerable(reader), (e) => false);
    }

    public static class Program
    {
        private static void WriteOutputLine(string text) => Console.WriteLine(text);

        private static bool HandleNoFilename(IndexOutOfRangeException ex)
        {
            WriteOutputLine("ERROR: No file name given.");
            return true;
        }

        public static Maybe<string> FilenameFromArgs(string[] args, Func<IndexOutOfRangeException, bool> handler) =>
            Fn.Try<string>(
                () => args[0],
                Fn.Handler<IndexOutOfRangeException>(handler)
            );

        public static IEnumerable<Account> AccountNumbersFromTextLines(IEnumerable<string> lines)
        {
            using var enlines = lines.GetEnumerator();

            do
            {
                var maybeTop = enlines.Next();

                // End of file
                if (!maybeTop)
                    yield break;

                var top = maybeTop.Value;
                var middle = enlines.Next().Value;
                var bottom = enlines.Next().Value;

                var account = GetNineDigits(top, middle, bottom).FromDigits();

                yield return account;

            } while (enlines.Next());
        }

        public static IEnumerable<Digit> GetNineDigits(string top, string middle, string bottom)
        {
            for (int i = 0; i < 9; i++)
            {
                Func<string, string> ThreeCharsAtOffset = (s => ThreeCharsAt(s, OffsetFromIndex(i)));

                var maybeStrings = new [] { ThreeCharsAtOffset(top), ThreeCharsAtOffset(middle), ThreeCharsAtOffset(bottom) };
                var rowToDigitMaps = new Func<string, Digit> [] { TopRowToDigit, MiddleRowToDigit, BottomRowToDigit };

                yield return maybeStrings
                    .Zip(rowToDigitMaps)
                    .Select(MapRowToDigit)
                    .Aggregate(Digit.All, (accumulator, digit) => accumulator * digit);
            }

            static int OffsetFromIndex(int index) => index * 4;
            static string ThreeCharsAt(string s, int offset) => s.Substring(offset, 3);
            static Digit MapRowToDigit((string row, Func<string, Digit> rowToDigit) pair) => pair.rowToDigit(pair.row);

            static Digit TopRowToDigit(string topRow) =>
                topRow switch
                {
                    " _ " => Digit.Zero + Digit.Two + Digit.Three + Digit.Five +
                             Digit.Six + Digit.Seven + Digit.Eight + Digit.Nine,
                    "   " => Digit.One + Digit.Four,
                    _ => throw new ArgumentException($"{nameof(topRow)} contans an invalid pattern: '{topRow}'")
                };

            static Digit MiddleRowToDigit(string middleRow) =>
                middleRow switch
                {
                    "| |" => Digit.Zero,
                    "  |" => Digit.One + Digit.Seven,
                    " _|" => Digit.Two + Digit.Three,
                    "|_|" => Digit.Four + Digit.Eight + Digit.Nine,
                    "|_ " => Digit.Five + Digit.Six,
                    _ => throw new ArgumentException($"{nameof(middleRow)} contans an invalid pattern: '{middleRow}'")
                };

            static Digit BottomRowToDigit(string bottomRow) =>
                bottomRow switch
                {
                    "|_|" => Digit.Zero + Digit.Six + Digit.Eight,
                    "  |" => Digit.One + Digit.Four + Digit.Seven,
                    "|_ " => Digit.Two,
                    " _|" => Digit.Three + Digit.Five + Digit.Nine,
                    _ => throw new ArgumentException($"{nameof(bottomRow)} contans an invalid pattern: '{bottomRow}'")
                };
        }

        public static void Main(string[] args)
        {
            // User Story 1: args -> displayed list of account numbers

            // * args -> filename
            var maybeFilename = FilenameFromArgs(args, HandleNoFilename);

            // * filename -> enumeration of text lines

            // enumeration of text lines -> enumeration of account numbers

            // * display account numbers

        }
    }
}
