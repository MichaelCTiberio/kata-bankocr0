using BankOcr.Domain;
using FunLib;
using System;
using System.Collections.Generic;
using System.IO;

using static BankOcr.Cli.FileReader;
using static BankOcr.Cli.Output;

namespace BankOcr.Cli
        {
    public static class Program
    {
        public static Maybe<string> ToFilename(this string[] args) =>
            Fn.Try<string>
            (
                () => args[0],
                Fn.Handler<IndexOutOfRangeException>(() => true)
            )
            .Bind(ToValidFilename);

        public static Maybe<string> ToValidFilename(this string filename) =>
            string.IsNullOrWhiteSpace(filename) ?
                Maybe<string>.None :
                Path.GetFullPath(filename);

        public static Maybe<TextReader> OpenFile(string filename, Func<bool> invalidFileNameHandler) =>
            Fn.Try<TextReader>
            (
                () => new StreamReader(filename),
                Fn.Handler<FileNotFoundException>(invalidFileNameHandler),
                Fn.Handler<DirectoryNotFoundException>(invalidFileNameHandler),
                Fn.Handler<IOException>(invalidFileNameHandler)
            );

        public static int Main(string[] args)
        {
            const int StatusSuccess = 0;
            const int StatusError = 1;

            int status = StatusSuccess;

            Writer successWriter = WriteToNull;
            Writer failureWriter = WriteLineToConsole;
            Writer outputWriter = WriteLineToConsole;

            // User Story 1: args -> displayed list of account numbers
            Maybe<string> maybeFilename =
                args
                    .ToFilename()
                    .ReportOnFilename(successWriter, failureWriter);

            if (!maybeFilename)
                return StatusError;

            Maybe<IEnumerable<string>> maybeLines =
                maybeFilename
                    .Bind(ToLines)
                    .ReportOnFile(successWriter, failureWriter);

            if (!maybeLines)
                return StatusError;

            Maybe<IEnumerable<Account>> maybeAccounts =
                maybeLines
                    .Map(FileReader.ToAccounts)
                    // * display account numbers
                    ;

            return status;
        }
    }
}
