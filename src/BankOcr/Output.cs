using System;
using System.Collections.Generic;
using FunLib;

namespace BankOcr.Cli
{
    public static class Output
    {
        public delegate string Writer(string text, object?[]? args = null);

        public static string WriteLineToConsole(string text, params object?[]? args)
        {
            Console.WriteLine(text, args);
            return text;
        }

        public static string WriteToNull(string text, params object?[]? _) => text;

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
}
