using System;
using System.Collections.Generic;
using System.IO;

namespace BankOcr
{
    public struct Result<T, TError>
    {
        public T Value { get; private set; }
        public TError Error { get; private set; }

        public bool Success { get; private set; }
        public static implicit operator bool(Result<T, TError> result) => result.Success;

        public static implicit operator Result<T, TError>(T value) => Result<T, TError>.Wrap(value);
        public static Result<T, TError> Wrap(T value) =>
            new Result<T, TError> { Value = value, Error = default, Success = true };

        public static implicit operator Result<T, TError>(TError error) => Result<T, TError>.Wrap(error);
        public static Result<T, TError> Wrap(TError error) =>
            new Result<T, TError> { Value = default, Error = error, Success = false };

        public static explicit operator Maybe<T>(Result<T, TError> result) =>
            (result.Success ? result.Value : Maybe<T>.None);

        public static Result<T, Exception> Catch(Func<T> f)
        {
            try
            {
                return f();
            }
            catch (Exception e)
            {
                return e;
            }
        }
    }

    public struct Maybe<T>
    {
        public T Value { get; private set; }
        public static explicit operator T(Maybe<T> maybe) => maybe.Value;

        public bool HasValue { get; private set; }
        public static implicit operator bool(Maybe<T> maybe) => maybe.HasValue;

        public static implicit operator Maybe<T>(T value) => Maybe<T>.Wrap(value);
        public static Maybe<T> Wrap(T value) =>
            new Maybe<T> { Value = value, HasValue = (value != null) };

        public static Maybe<T> None { get; } =
            new Maybe<T> { Value = default, HasValue = false };
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
        private static IEnumerable<string> LinesEnumerable(TextReader reader)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
                yield return line;
        }

        public static Maybe<IEnumerable<string>> Lines(TextReader reader) =>
            (Maybe<IEnumerable<string>>)
                Result<IEnumerable<string>, Exception>.Catch(() => LinesEnumerable(reader));
    }

    public static class Utility
    {
        static public Maybe<T> MaybeIs<T>(this object obj)
        {
            if (obj is T output)
                return output;

            return Maybe<T>.None;
        }

    }

    static class Program
    {
        static void Main(string[] args)
        {
            // string(filename) -> TextReader(a StreamReader opened to the file)
            // Lines: TextReader -> Result<IEnumerable<string>, Exception>(lines from file)
        }
    }
}
