using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BankOcr.Domain;
using FunLib;
using static BankOcr.Cli.Output;

namespace BankOcr.Cli;

public static class FileReader
{
    public static Maybe<IEnumerable<string>> ToLines(this string filename) =>
        Fn.Try<IEnumerable<string>>
        (
            () => File.ReadLines(filename),
            Fn.Handler<Exception>(() => true)
        );

    public static IEnumerable<Account> ToAccounts(IEnumerable<string> lines)
    {
        using var enlines = lines.GetEnumerator();

        var maybeRows = GetFirstThreeRows(enlines);
        while (maybeRows)
        {
            yield return GetNineDigits(maybeRows.Value.Top, maybeRows.Value.Middle, maybeRows.Value.Bottom).FromDigits();
            maybeRows = GetNextThreeRows(enlines);
        }

        static IEnumerable<Digit> GetNineDigits(string top, string middle, string bottom) =>
            DoNineTimes((index) => GetOneDigit(index, top, middle, bottom));

        static Digit GetOneDigit(int index, string top, string middle, string bottom)
        {
            string [] lines = new [] { top, middle, bottom };
            var rowsToDigits = new Func<string, Digit> [] { TopRowToDigit, MiddleRowToDigit, BottomRowToDigit };

            return lines
                .ToRows(index)
                .Zip(rowsToDigits)
                .Select(MapRowsToDigits)
                .Aggregate(Digit.Any, (accumulator, digit) => accumulator & digit);
        }

        static Maybe<(string Top, string Middle, string Bottom)> GetFirstThreeRows(IEnumerator<string> enlines)
        {
            var maybeTop = enlines.Next();
            return maybeTop ?
                (maybeTop.Value, enlines.Next().Value, enlines.Next().Value) :
                Maybe<(string Top, string Middle, string Bottom)>.None;
        }

        static Maybe<(string Top, string Middle, string Bottom)> GetNextThreeRows(IEnumerator<string> enlines)
        {
            DiscardEmptyRow(enlines);
            return GetFirstThreeRows(enlines);
        }

        static void DiscardEmptyRow(IEnumerator<string> enlines) =>
            enlines.Next();

        static Digit MapRowsToDigits((string row, Func<string, Digit> rowToDigit) pair) =>
            pair.rowToDigit(pair.row);

        static IEnumerable<T> DoNineTimes<T>(Func<int, T> func)
        {
            for (int i = 0; i < 9; i++)
                yield return func(i);
        }

        static Digit TopRowToDigit(string topRow) =>
            topRow switch
            {
                " _ " => Digit.Zero | Digit.Two | Digit.Three | Digit.Five |
                            Digit.Six | Digit.Seven | Digit.Eight | Digit.Nine,
                "   " => Digit.One | Digit.Four,
                _ => throw new ArgumentException($"Invalid pattern [{topRow}]", nameof(topRow))
            };

        static Digit MiddleRowToDigit(string middleRow) =>
            middleRow switch
            {
                "| |" => Digit.Zero,
                "  |" => Digit.One | Digit.Seven,
                " _|" => Digit.Two | Digit.Three,
                "|_|" => Digit.Four | Digit.Eight | Digit.Nine,
                "|_ " => Digit.Five | Digit.Six,
                _ => throw new ArgumentException($"Invalid pattern [{middleRow}]", nameof(middleRow))
            };

        static Digit BottomRowToDigit(string bottomRow) =>
            bottomRow switch
            {
                "|_|" => Digit.Zero | Digit.Six | Digit.Eight,
                "  |" => Digit.One | Digit.Four | Digit.Seven,
                "|_ " => Digit.Two,
                " _|" => Digit.Three | Digit.Five | Digit.Nine,
                _ => throw new ArgumentException($"Invalid pattern [{bottomRow}]", nameof(bottomRow))
            };
    }

    private static IEnumerable<string> ToRows(this IEnumerable<string> lines, int digitIndex)
    {
        return lines
            .Select((line) =>
                ThreeCharsAt(line, ToLineIndex(digitIndex)));

        static int ToLineIndex(int index) =>
            index * 3;

        static string ThreeCharsAt(string s, int offset) =>
            s.Substring(offset, 3);
    }

    public static Maybe<string> ReportOnFilename(this Maybe<string> maybeFilename, Writer successWriter, Writer failureWriter)
    {
        const string haveFilenameReport = "Attempting to read file: {0}";
        const string emptyFilenameReport = "ERROR: No file name given.";

        return maybeFilename
            .HaveAction((filename) => successWriter(haveFilenameReport, new [] { filename }))
            .EmptyAction(() => failureWriter(emptyFilenameReport));
    }

    public static Maybe<IEnumerable<string>> ReportOnFile(this Maybe<IEnumerable<string>> maybeLines, Writer successWriter, Writer failureWriter)
    {
        const string haveLinesReport = "File opened.";
        const string emptyLinesReport = "ERROR: Could not open file.";

        return maybeLines
            .HaveAction(() => successWriter(haveLinesReport))
            .EmptyAction(() => failureWriter(emptyLinesReport));
    }
}
