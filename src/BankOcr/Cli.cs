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
            (args.Length == 0 ?
                Maybe<string>.None :
                Maybe<string>.Wrap(args[0])
            )
            .Bind((argument) =>
                string.IsNullOrWhiteSpace(argument) ?
                    Maybe<string>.None :
                    Path.GetFullPath(argument)
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
            args
            .ToFilename()
            .ReportOnFilename(successWriter, failureWriter)
            .Bind((filename) =>
                filename
                .OpenFile()
                .ReportOnFile(successWriter, failureWriter))
            .Map((reader) =>
                reader
                .ToLines()
                .ToAccounts())
            .EmptyAction(SetErrorStatus);

            return status;

            void SetErrorStatus() => status = StatusError;
        }
    }
}
