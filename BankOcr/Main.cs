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
        private T Value { get; set; }
        private bool HasValue { get; set; }

        public static implicit operator Maybe<T>(T value) =>
            new Maybe<T> { Value = value, HasValue = (value != null) };

        public static Maybe<T> None = new Maybe<T>();
    }

    public struct Digit
    {
        public char Value { get; private set; }

        public static Result<Digit> MaybeFromString(string s) =>
            s switch
            {
                Zero => Result<Digit>.NewResult(new Digit { Value = '0' }),
                One => Result<Digit>.NewResult(new Digit { Value = '1' }),
                Two => Result<Digit>.NewResult(new Digit { Value = '2' }),
                Three => Result<Digit>.NewResult(new Digit { Value = '3' }),
                Four => Result<Digit>.NewResult(new Digit { Value = '4' }),
                Five => Result<Digit>.NewResult(new Digit { Value = '5' }),
                Six => Result<Digit>.NewResult(new Digit { Value = '6' }),
                Seven => Result<Digit>.NewResult(new Digit { Value = '7' }),
                Eight => Result<Digit>.NewResult(new Digit { Value = '8' }),
                Nine => Result<Digit>.NewResult(new Digit { Value = '9' }),
                _ => Result<Digit>.NewError($"Invalid string \"{s}\""),
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
