using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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

        private readonly bool hasValue;
        public static implicit operator bool(Maybe<T> maybe) => maybe.hasValue;

        private Maybe(T value, bool hasValue)
        {
            this.Value = value;
            this.hasValue = hasValue;
        }

        public static implicit operator Maybe<T>(T value) => Maybe<T>.Wrap(value);
        public static Maybe<T> Wrap(T value) => new (value: value, hasValue: (value != null));

        public static Maybe<T> None { get; } = new (value: default, hasValue: false);

        public readonly bool HasValue() => hasValue;
        public static bool HasValue(Maybe<T> maybe) => maybe.HasValue();

        public readonly Maybe<TReturn> Map<TReturn>(Func<T, TReturn> f) =>
            hasValue ?
                f(Value) :
                Maybe<TReturn>.None;

        public override string ToString() =>
            (hasValue) ? Value.ToString() : "<empty>";
    }

    public static class MaybeHelpers
    {
        public static Maybe<IEnumerable<T>> MaybeEnumerable<T>(this IEnumerable<Maybe<T>> maybes)
        {
            return maybes.All(Maybe<T>.HasValue) ?
                Maybe<IEnumerable<T>>.Wrap(EnumerableFromEnumerableMaybe(maybes)) :
                Maybe<IEnumerable<T>>.None;

            // The following function assumes that all the Maybe<T> items have a value.
            // That is dangerous in general, so we hide it in the method scope.
            static IEnumerable<T> EnumerableFromEnumerableMaybe(IEnumerable<Maybe<T>> maybes)
            {
                foreach (var item in maybes)
                    yield return item.Value;
            }
        }

    }

    public readonly struct Digit
    {
        private readonly char value;
        public static implicit operator char(Digit digit) => digit.value;

        private Digit(char value) => this.value = value;

        public static Digit FromChar(char c) =>
            c switch
            {
                >= '0' and <= '9' => new Digit(c),
                _ => throw new NotImplementedException("Invalid char"),
            };

        public static Digit FromString(string s) =>
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
                _ => throw new NotImplementedException("Invalid string"),
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

        public static string Top(Digit d) =>
            d.value switch
            {
                '0' => Zero[0..3],
                '1' => One[0..3],
                '2' => Two[0..3],
                '3' => Three[0..3],
                '4' => Four[0..3],
                '5' => Five[0..3],
                '6' => Six[0..3],
                '7' => Seven[0..3],
                '8' => Eight[0..3],
                '9' => Nine[0..3],
                _ => throw new InvalidOperationException("Digit object is not valid")
            };

        public static string Middle(Digit d) =>
            d.value switch
            {
                '0' => Zero[3..6],
                '1' => One[3..6],
                '2' => Two[3..6],
                '3' => Three[3..6],
                '4' => Four[3..6],
                '5' => Five[3..6],
                '6' => Six[3..6],
                '7' => Seven[3..6],
                '8' => Eight[3..6],
                '9' => Nine[3..6],
                _ => throw new InvalidOperationException("Digit object is not valid")
            };

        public static string Bottom(Digit d) =>
            d.value switch
            {
                '0' => Zero[6..9],
                '1' => One[6..9],
                '2' => Two[6..9],
                '3' => Three[6..9],
                '4' => Four[6..9],
                '5' => Five[6..9],
                '6' => Six[6..9],
                '7' => Seven[6..9],
                '8' => Eight[6..9],
                '9' => Nine[6..9],
                _ => throw new InvalidOperationException("Digit object is not valid")
            };

        public override string ToString() => value.ToString();
    }

    public readonly struct Account
    {
        public string Number { get; init; }

        public static implicit operator string(Account account) => account.Number;

        public static Account FromDigits(IEnumerable<Digit> digits)
        {
            using var endigits = digits.GetEnumerator();

            endigits.MoveNext();
            char digit1 = endigits.Current;
            endigits.MoveNext();
            char digit2 = endigits.Current;
            endigits.MoveNext();
            char digit3 = endigits.Current;
            endigits.MoveNext();
            char digit4 = endigits.Current;
            endigits.MoveNext();
            char digit5 = endigits.Current;
            endigits.MoveNext();
            char digit6 = endigits.Current;
            endigits.MoveNext();
            char digit7 = endigits.Current;
            endigits.MoveNext();
            char digit8 = endigits.Current;
            endigits.MoveNext();
            char digit9 = endigits.Current;

            string number = $"{digit1}{digit2}{digit3}" +
                            $"{digit4}{digit5}{digit6}" +
                            $"{digit7}{digit8}{digit9}";

            return new Account { Number = number };
        }

        public override string ToString() => Number;
    }

    public static class FileReader
    {
        private static IEnumerable<string> LinesEnumerable(TextReader reader)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
                yield return line;
        }

        // TODO: Should not be using Catch
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
            try
            {
                 return f();
            }
            catch (Exception ex)
            {
                if (!handlers.Any((handler) => handler(ex)))
                    throw;
            }

            return Maybe<T>.None;
        }

        public static Maybe<T> Try<T>(Func<Maybe<T>> func, params Func<Exception, bool> [] handlers) =>
            Try(func, (IEnumerable<Func<Exception, bool>>) handlers);

        public static Func<Exception, bool> Handler<TException>(Func<TException, bool> handler)
            where TException : Exception =>
            (Exception exception) =>
            {
                var maybeException = exception.MaybeIs<TException>();
                return maybeException ?
                    handler((TException) maybeException) :
                    false;
            };

        public static T Use<TDisposable, T>(TDisposable disposable, Func<TDisposable, T> func)
            where TDisposable : IDisposable
        {
            using TDisposable resource = disposable;
            return func(resource);
        }
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
