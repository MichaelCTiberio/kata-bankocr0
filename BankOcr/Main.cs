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
            new Roe<T> { Valid = true, Result = result, Error = "" };

        public static Roe<T> NewError(string error) =>
            new Roe<T> { Valid = false, Result = default, Error = error };
    }

    public static class DigitStrings
    {
        // " _ "
        // "| |"
        // "|_|"
        public const string Zero = " _ | ||_|";
        // "   "
        // "  |"
        // "  |"
        public const string One = "     |  |";
        // " _ "
        // " _|"
        // "|_ "
        public const string Two = " _  _||_ ";
        // " _ "
        // " _|"
        // " _|"
        public const string Three = " _  _| _|";
        // "   "
        // "|_|"
        // "  |"
        public const string Four = "   |_|  |";
        // " _ "
        // "|_ "
        // " _|"
        public const string Five = " _ |_  _|";
        // " _ "
        // "|_ "
        // "|_|"
        public const string Six = " _ |_ |_|";
        // " _ "
        // "  |"
        // "  |"
        public const string Seven = " _   |  |";
        // " _ "
        // "|_|"
        // "|_|"
        public const string Eight = " _ |_||_|";
        // " _ "
        // "|_|"
        // "  |"
        public const string Nine = " _ |_| _|";
    }

    public struct Digit
    {
        public char Value { get; }

        private Digit(char c) => Value = c;

        // private static Digit? MaybeFromChar(char c)
        // {
        //     return c switch
        //     {
        //         (>= '0') and (<= '9') => new Digit(c),
        //         _ => null,
        //     };
        // }

        public static Roe<Digit> MaybeFromString(string s)
        {
            return s switch
            {
                DigitStrings.Zero => Roe<Digit>.NewResult(new Digit('0')),
                DigitStrings.One => Roe<Digit>.NewResult(new Digit('1')),
                DigitStrings.Two => Roe<Digit>.NewResult(new Digit('2')),
                DigitStrings.Three => Roe<Digit>.NewResult(new Digit('3')),
                DigitStrings.Four => Roe<Digit>.NewResult(new Digit('4')),
                DigitStrings.Five => Roe<Digit>.NewResult(new Digit('5')),
                DigitStrings.Six => Roe<Digit>.NewResult(new Digit('6')),
                DigitStrings.Seven => Roe<Digit>.NewResult(new Digit('7')),
                DigitStrings.Eight => Roe<Digit>.NewResult(new Digit('8')),
                DigitStrings.Nine => Roe<Digit>.NewResult(new Digit('9')),
                _ => Roe<Digit>.NewError($"Invalid string \"{s}\""),
            };
        } 
    }

    static class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
