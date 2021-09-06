using System.Collections.Generic;
using System.IO;

namespace BankOcr
{
    public struct Result<T, TError>
    {
        private T value;
        public static explicit operator T(Result<T, TError> result) => result.value;

        private TError error;
        public static explicit operator TError(Result<T, TError> result) => result.error;

        private bool success;
        public static implicit operator bool(Result<T, TError> result) => result.success;

        public static implicit operator Result<T, TError>(T value) =>
            new Result<T, TError> { value = value, error = default, success = true };

        public static implicit operator Result<T, TError>(TError error) =>
            new Result<T, TError> { value = default, error = error, success = false };
    }

    public struct Maybe<T>
    {
        private T value;
        public static explicit operator T(Maybe<T> maybe) => maybe.value;

        private bool hasValue;
        public static implicit operator bool(Maybe<T> maybe) => maybe.hasValue;

        public static implicit operator Maybe<T>(T value) =>
            new Maybe<T> { value = value, hasValue = (value != null) };

        public static Maybe<T> None { get; } = new Maybe<T>();
    }

    public struct Digit
    {
        private char value;
        public static implicit operator char(Digit digit) => digit.value;

        public static Maybe<Digit> FromString(string s) =>
            s switch
            {
                Zero => new Digit { value = '0' },
                One => new Digit { value = '1' },
                Two => new Digit { value = '2' },
                Three => new Digit { value = '3' },
                Four => new Digit { value = '4' },
                Five => new Digit { value = '5' },
                Six => new Digit { value = '6' },
                Seven => new Digit { value = '7' },
                Eight => new Digit { value = '8' },
                Nine => new Digit { value = '9' },
                _ => Maybe<Digit>.None,
            };

        public const string Zero =
            " _ " +
            "| |" +
            "|_|";
        public const string One = 
            "   " +
            "  |" +
            "  |";
        public const string Two = 
            " _ " +
            " _|" +
            "|_ ";
        public const string Three = 
            " _ " +
            " _|" +
            " _|";
        public const string Four = 
            "   " +
            "|_|" +
            "  |";
        public const string Five = 
            " _ " +
            "|_ " +
            " _|";
        public const string Six = 
            " _ " +
            "|_ " +
            "|_|";
        public const string Seven = 
            " _ " +
            "  |" +
            "  |";
        public const string Eight = 
            " _ " +
            "|_|" +
            "|_|";
        public const string Nine = 
            " _ " +
            "|_|" +
            " _|";
    }

    public static class FileReader
    {
        public static IEnumerable<string> Lines(TextReader reader)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
                yield return line;
        }
    }

    static class Program
    {
        static void Main(string[] args)
        {
            // string(filename) -> TextReader(a StreamReader opened to the file)
            // Lines: TextReader -> IEnumerable<string>(lines from file)
        }
    }
}
