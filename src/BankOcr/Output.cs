using System;
using System.Collections.Generic;
using System.IO;
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
    }
}
