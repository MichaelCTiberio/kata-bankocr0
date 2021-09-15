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

        public bool HasValue { readonly get; init; }
        public static implicit operator bool(Maybe<T> maybe) => maybe.HasValue;

        public static implicit operator Maybe<T>(T value) => Maybe<T>.Wrap(value);
        public static Maybe<T> Wrap(T value) => new () { Value = value, HasValue = (value != null) };

        public static Maybe<T> None { get; } = new () { Value = default, HasValue = false };

        public override string ToString() =>
            (HasValue) ? Value.ToString() : "<empty>";
    }

    public readonly struct Digit
    {
        private readonly char value;
        public static implicit operator char(Digit digit) => digit.value;

        private Digit(char value) => this.value = value;

        public static Maybe<Digit> FromChar(char c) =>
            c switch
            {
                >= '0' and <= '9' => new Digit(c),
                _ => Maybe<Digit>.None,
            };

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

        public readonly string Top
        {
            get
            {
                string result = 
                value switch
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
                return result;
            }
        }

        public readonly string Middle
        {
            get
            {
                string result = 
                value switch
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
                return result;
            }
        }

        public readonly string Bottom
        {
            get
            {
                string result =
                value switch
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
                return result;
            }
        }

        public override string ToString() => value.ToString();
    }

    public readonly struct Account
    {
        public string Number { get; init; }

        public static implicit operator string(Account account) => account.Number;

        public static Maybe<Account> FromDigits(IEnumerable<Digit> digits)
        {
            using var endigits = digits.GetEnumerator();

            if (!endigits.MoveNext()) return Maybe<Account>.None;
            char digit1 = endigits.Current;
            if (!endigits.MoveNext()) return Maybe<Account>.None;
            char digit2 = endigits.Current;
            if (!endigits.MoveNext()) return Maybe<Account>.None;
            char digit3 = endigits.Current;
            if (!endigits.MoveNext()) return Maybe<Account>.None;
            char digit4 = endigits.Current;
            if (!endigits.MoveNext()) return Maybe<Account>.None;
            char digit5 = endigits.Current;
            if (!endigits.MoveNext()) return Maybe<Account>.None;
            char digit6 = endigits.Current;
            if (!endigits.MoveNext()) return Maybe<Account>.None;
            char digit7 = endigits.Current;
            if (!endigits.MoveNext()) return Maybe<Account>.None;
            char digit8 = endigits.Current;
            if (!endigits.MoveNext()) return Maybe<Account>.None;
            char digit9 = endigits.Current;

            // Check to make sure that there are no more digits.
            if (endigits.MoveNext()) return Maybe<Account>.None;

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

        public static Func<Exception, bool> Handler<TException>(Func<TException, bool> handler)
            where TException : Exception =>
            (Exception exception) =>
            {
                var maybeException = exception.MaybeIs<TException>();
                return maybeException ?
                    handler((TException) maybeException) :
                    false;
            };

        public static Maybe<T> Use<TDisposable, T>(TDisposable disposable, Func<TDisposable, T> f)
            where TDisposable : IDisposable
        {
            using TDisposable resource = disposable;
            return f(resource);
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

        public static Maybe<IEnumerable<Account>> AccountNumbersFromTextLines(IEnumerable<string> rows)
        {
            LinkedList<Account> accounts = new ();

            using var enlines = rows.GetEnumerator();

            while (enlines.MoveNext())
            {
                string rowTop = enlines.Current;
                if (!enlines.MoveNext()) return Maybe<IEnumerable<Account>>.None;
                string rowMiddle = enlines.Current;
                if (!enlines.MoveNext()) return Maybe<IEnumerable<Account>>.None;
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
                    if (digits.Count == 9) return Maybe<IEnumerable<Account>>.None;

                    var first = encols.Current;
                    if (!encols.MoveNext()) return Maybe<IEnumerable<Account>>.None;
                    var second = encols.Current;
                    if (!encols.MoveNext()) return Maybe<IEnumerable<Account>>.None;
                    var third = encols.Current;

                    // throw one column away, may not be present at the end of the account number
                    encols.MoveNext();

                    string s = $"{first.top   }{second.top   }{third.top   }" +
                               $"{first.middle}{second.middle}{third.middle}" +
                               $"{first.bottom}{second.bottom}{third.bottom}";

                    var maybeDigit = Digit.FromString(s);
                    if (!maybeDigit) return Maybe<IEnumerable<Account>>.None;
                    digits.Add((Digit) maybeDigit);
                }

                var maybeAccount = Account.FromDigits((IEnumerable<Digit>) digits);
                if (!maybeAccount) return Maybe<IEnumerable<Account>>.None;
                accounts.AddLast((Account) maybeAccount);
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
