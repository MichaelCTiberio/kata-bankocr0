using System;
using System.Collections.Generic;
using System.Linq;

namespace BankOcr.Domain
{
    public struct Digit
    {
        private enum DigitMask
        {
            Zero = 1 << 0,
            One = 1 << 1,
            Two = 1 << 2,
            Three = 1 << 3,
            Four = 1 << 4,
            Five = 1 << 5,
            Six = 1 << 6,
            Seven = 1 << 7,
            Eight = 1 << 8,
            Nine = 1 << 9,
            All = Zero | One | Two | Three | Four | Five | Six | Seven | Eight | Nine,
        }

        DigitMask mask { get; init; }

        public static Digit operator +(Digit lhs, Digit rhs) => new Digit { mask = lhs.mask | rhs.mask };

        public static Digit operator *(Digit lhs, Digit rhs) => new Digit { mask = lhs.mask & rhs.mask };

        public static explicit operator char(Digit digit) =>
            digit.mask switch
            {
                DigitMask.Zero => '0',
                DigitMask.One => '1',
                DigitMask.Two => '2',
                DigitMask.Three => '3',
                DigitMask.Four => '4',
                DigitMask.Five => '5',
                DigitMask.Six => '6',
                DigitMask.Seven => '7',
                DigitMask.Eight => '8',
                DigitMask.Nine => '9',
                _ => throw new InvalidOperationException($"{nameof(Digit)} is in an invalid state: {digit.mask:X}")
            };

        public static Digit Zero = new Digit { mask = DigitMask.Zero };
        public static Digit One = new Digit { mask = DigitMask.One };
        public static Digit Two = new Digit { mask = DigitMask.Two };
        public static Digit Three = new Digit { mask = DigitMask.Three };
        public static Digit Four = new Digit { mask = DigitMask.Four };
        public static Digit Five = new Digit { mask = DigitMask.Five };
        public static Digit Six = new Digit { mask = DigitMask.Six };
        public static Digit Seven = new Digit { mask = DigitMask.Seven };
        public static Digit Eight = new Digit { mask = DigitMask.Eight };
        public static Digit Nine = new Digit { mask = DigitMask.Nine }; 
        public static Digit All = new Digit { mask = DigitMask.All };
    }

    public readonly struct Account
    {
        public string Number { get; init; }

        public static implicit operator string(Account account) => account.Number;

        public override string ToString() => Number;
    }

    public static class AccountHelpers
    {
        public static Account FromDigits(this IEnumerable<Digit> digits) =>
            new Account
            {
                Number = (new System.Text.StringBuilder()).AppendJoin("", digits.Select(digit => (char) digit)).ToString()
            };
    }
}
