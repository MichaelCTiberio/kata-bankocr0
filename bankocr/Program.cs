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
                "     |  |" => new Digit('1'),
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
