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

        public static IEnumerable<Digit2> GetNineDigits(string top, string middle, string bottom)
        {
            for (int i = 0; i < 9; i++)
            {
                Func<string, string> ThreeCharsAtOffset = (s => ThreeCharsAt(s, OffsetFromIndex(i)));

                var maybeStrings = new [] { ThreeCharsAtOffset(top), ThreeCharsAtOffset(middle), ThreeCharsAtOffset(bottom) };
                var rowToDigitMaps = new Func<string, Digit2> [] { TopRowToDigit, MiddleRowToDigit, BottomRowToDigit };

                yield return maybeStrings
                    .Zip(rowToDigitMaps)
                    .Select(MapRowToDigit)
                    .Aggregate(Digit2.All, (accumulator, digit) => accumulator * digit);
            }

            static int OffsetFromIndex(int index) => index * 4;
            static string ThreeCharsAt(string s, int offset) => s.Substring(offset, 3);
            static Digit2 MapRowToDigit((string row, Func<string, Digit2> rowToDigit) pair) => pair.rowToDigit(pair.row);

            static Digit2 TopRowToDigit(string topRow) =>
                topRow switch
                {
                    " _ " => Digit2.Zero + Digit2.Two + Digit2.Three + Digit2.Five +
                             Digit2.Six + Digit2.Seven + Digit2.Eight + Digit2.Nine,
                    "   " => Digit2.One + Digit2.Four,
                    _ => throw new ArgumentException($"{nameof(topRow)} contans an invalid pattern: '{topRow}'")
                };

            static Digit2 MiddleRowToDigit(string middleRow) =>
                middleRow switch
                {
                    "| |" => Digit2.Zero,
                    "  |" => Digit2.One + Digit2.Seven,
                    " _|" => Digit2.Two + Digit2.Three,
                    "|_|" => Digit2.Four + Digit2.Eight + Digit2.Nine,
                    "|_ " => Digit2.Five + Digit2.Six,
                    _ => throw new ArgumentException($"{nameof(middleRow)} contans an invalid pattern: '{middleRow}'")
                };

            static Digit2 BottomRowToDigit(string bottomRow) =>
                bottomRow switch
                {
                    "|_|" => Digit2.Zero + Digit2.Six + Digit2.Eight,
                    "  |" => Digit2.One + Digit2.Four + Digit2.Seven,
                    "|_ " => Digit2.Two,
                    " _|" => Digit2.Three + Digit2.Five + Digit2.Nine,
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
