using System;

namespace BankOcr
{
    public struct Result<T>
    {
        public T Value { get; private set; }
        public string Error { get; private set; }
        public bool Valid { get; private set; }

        public static Result<T> NewResult(T result) =>
            new Result<T> { Valid = true, Value = result, Error = default };

        public static Result<T> NewError(string error) =>
            new Result<T> { Valid = false, Value = default, Error = error };

        public Result<T> Invoke(Func<T, Result<T>> f) => (Valid ? f(Value) : this);

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

    static class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
