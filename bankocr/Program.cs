using System;

namespace BankOcr
{
    public struct Digit
    {
        public char Value { get; }

        private Digit(char c) => Value = c;

        public static Digit? MaybeNew(char c)
        {
            return c switch
            {
                (>= '0') and (<= '9') => new Digit(c),
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
