using System;

namespace BankOcr
{
    /// <summary>Result or Error</summary>
    public struct Roe<T>
    {
        public T Result { get; private set; }
        public string Error { get; private set; }
        public bool Valid { get; private set; }

        public static Roe<T> NewResult(T result) =>
            new Roe<T> { Valid = true, Result = result, Error = default };

        public static Roe<T> NewError(string error) =>
            new Roe<T> { Valid = false, Result = default, Error = error };

        public Roe<T> Invoke(Func<T, Roe<T>> f) => (Valid ? f(Result) : this); 
    }

    public struct Digit
    {
        public char Value { get; private set; }

        public static Roe<Digit> MaybeFromString(string s) =>
            s switch
            {
                Zero => Roe<Digit>.NewResult(new Digit { Value = '0' }),
                One => Roe<Digit>.NewResult(new Digit { Value = '1' }),
                Two => Roe<Digit>.NewResult(new Digit { Value = '2' }),
                Three => Roe<Digit>.NewResult(new Digit { Value = '3' }),
                Four => Roe<Digit>.NewResult(new Digit { Value = '4' }),
                Five => Roe<Digit>.NewResult(new Digit { Value = '5' }),
                Six => Roe<Digit>.NewResult(new Digit { Value = '6' }),
                Seven => Roe<Digit>.NewResult(new Digit { Value = '7' }),
                Eight => Roe<Digit>.NewResult(new Digit { Value = '8' }),
                Nine => Roe<Digit>.NewResult(new Digit { Value = '9' }),
                _ => Roe<Digit>.NewError($"Invalid string \"{s}\""),
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
