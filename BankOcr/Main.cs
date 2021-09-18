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

        public static IEnumerable<Account> AccountNumbersFromTextLines(IEnumerable<string> rows)
        {
            LinkedList<Account> accounts = new ();

            using var enlines = rows.GetEnumerator();

            while (enlines.MoveNext())
            {
                string rowTop = enlines.Current;
                enlines.MoveNext();
                string rowMiddle = enlines.Current;
                enlines.MoveNext();
                string rowBottom = enlines.Current;

                // throw one row away, may not be present at the end of the file
                enlines.MoveNext();

                var cols = rowTop
                    .Zip(rowMiddle, (top, middle) => (top, middle))
                    .Zip(rowBottom, (item, bottom) => (item.top, item.middle, bottom));

                using var encols = cols.GetEnumerator();

                List<Digit> digits = new (9);

                while (encols.MoveNext())
                {
                    var first = encols.Current;
                    encols.MoveNext();
                    var second = encols.Current;
                    encols.MoveNext();
                    var third = encols.Current;

                    // throw one column away, may not be present at the end of the account number
                    encols.MoveNext();

                    string s = $"{first.top   }{second.top   }{third.top   }" +
                               $"{first.middle}{second.middle}{third.middle}" +
                               $"{first.bottom}{second.bottom}{third.bottom}";

                    digits.Add(Digit.FromString(s));
                }

                accounts.AddLast(Account.FromDigits((IEnumerable<Digit>) digits));
            }

            return accounts;
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
