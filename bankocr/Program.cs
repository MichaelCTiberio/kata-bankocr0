using System;

namespace BankOcr
{
    public struct Digit
    {
        public char Value { get; }

        private Digit(char c) => Value = c;

        public static Digit? MaybeFromChar(char c)
        {
            return c switch
            {
                (>= '0') and (<= '9') => new Digit(c),
                _ => null,
            };
        }

        public static Digit? MaybeFromString(string s)
        {
            return s switch
            {
                " _ | ||_|" => new Digit('0'),
                "     |  |" => new Digit('1'),
                " _  _||_ " => new Digit('2'),
                " _  _| _|" => new Digit('3'),
                "   |_|  |" => new Digit('4'),
                " _ |_  _|" => new Digit('5'),
                " _ |_ |_|" => new Digit('6'),
                " _   |  |" => new Digit('7'),
                " _ |_||_|" => new Digit('8'),
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
