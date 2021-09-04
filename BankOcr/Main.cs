using System;

namespace BankOcr
{
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

        public static Digit? MaybeFromString(string s)
        {
            return s switch
            {
                DigitStrings.Zero => new Digit('0'),
                DigitStrings.One => new Digit('1'),
                DigitStrings.Two => new Digit('2'),
                DigitStrings.Three => new Digit('3'),
                DigitStrings.Four => new Digit('4'),
                DigitStrings.Five => new Digit('5'),
                DigitStrings.Six => new Digit('6'),
                DigitStrings.Seven => new Digit('7'),
                DigitStrings.Eight => new Digit('8'),
                DigitStrings.Nine => new Digit('9'),
                _ => null,
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
