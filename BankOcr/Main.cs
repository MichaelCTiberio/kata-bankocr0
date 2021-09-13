using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BankOcr
{
    public readonly struct Result<T, TError>
    {
        public T Value { readonly get; init; }
        public TError Error { readonly get; init; }

        public bool Success { readonly get; init; }
        public static implicit operator bool(Result<T, TError> result) => result.Success;

        public static implicit operator Result<T, TError>(T value) => Result<T, TError>.Wrap(value);
        public static Result<T, TError> Wrap(T value) =>
            new () { Value = value, Error = default, Success = true };

        public static implicit operator Result<T, TError>(TError error) => Result<T, TError>.Wrap(error);
        public static Result<T, TError> Wrap(TError error) =>
            new () { Value = default, Error = error, Success = false };

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

    public readonly struct Maybe<T>
    {
        public T Value { readonly get; init; }
        public static explicit operator T(Maybe<T> maybe) => maybe.Value;

        public bool HasValue { readonly get; init; }
        public static implicit operator bool(Maybe<T> maybe) => maybe.HasValue;

        public static implicit operator Maybe<T>(T value) => Maybe<T>.Wrap(value);
        public static Maybe<T> Wrap(T value) => new () { Value = value, HasValue = (value != null) };

        public static Maybe<T> None { get; } = new () { Value = default, HasValue = false };
    }

    public readonly struct Digit
    {
        private readonly char value;
        public static implicit operator char(Digit digit) => digit.value;

        private Digit(char value) => this.value = value;

        public static Maybe<Digit> FromString(string s) =>
            s switch
            {
                Zero => new Digit('0'),
                One => new Digit('1'),
                Two => new Digit('2'),
                Three => new Digit('3'),
                Four => new Digit('4'),
                Five => new Digit('5'),
                Six => new Digit('6'),
                Seven => new Digit('7'),
                Eight => new Digit('8'),
                Nine => new Digit('9'),
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
        public static Maybe<T> MaybeIs<T>(this object obj)
        {
            if (obj is T output)
                return output;

            return Maybe<T>.None;
        }

        public static Maybe<T> Try<T>(Func<Maybe<T>> f, IEnumerable<Func<Exception, bool>> handlers)
        {
            Maybe<T> ret = Maybe<T>.None;

            try
            {
                 ret = f();
            }
            catch (Exception ex)
            {
                if (!handlers.Any((handler) => handler(ex)))
                    throw;
            }

            return ret;
        }

        public static Maybe<T> Try<T>(Func<Maybe<T>> f, params Func<Exception, bool> [] handlers) =>
            Try(f, (IEnumerable<Func<Exception, bool>>) handlers);

        public static Func<Exception, bool> Handler<TEx>(Func<TEx, bool> handler) where TEx : Exception =>
            (Exception ex) =>
            {
                bool ret = false;

                var maybeTex = ex.MaybeIs<TEx>();

                if (ex is TEx exception)
                    ret = handler(exception);

                return ret;
            };
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
            Utility.Try<string>(
                () => args[0],
                Utility.Handler<IndexOutOfRangeException>(handler)
            );

        public static void Main(string[] args)
        {
            // string(filename) -> TextReader(a StreamReader opened to the file)
            // Lines: TextReader -> Result<IEnumerable<string>, Exception>(lines from file)

            var filename = FilenameFromArgs(args, HandleNoFilename);

            if (filename)
                WriteOutputLine($"File name: {(string) filename}");

        }
    }
}
