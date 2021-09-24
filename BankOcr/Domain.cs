using System;
using System.Collections.Generic;
using System.Linq;

namespace BankOcr.Domain
{
    public readonly struct DigitBuilder
    {
        private readonly char value;

        private DigitBuilder(char value) => this.value = value;

        public static DigitBuilder FromChar(char c) =>
            c switch
            {
                >= '0' and <= '9' => new DigitBuilder(c),
                _ => throw new NotImplementedException("Invalid char"),
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

        public static string Top(DigitBuilder d) =>
            d.value switch
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

        public static string Middle(DigitBuilder d) =>
            d.value switch
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

        public static string Bottom(DigitBuilder d) =>
            d.value switch
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
    }

    public struct Digit2
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

        public static Digit2 operator +(Digit2 lhs, Digit2 rhs) => new Digit2 { mask = lhs.mask | rhs.mask };

        public static Digit2 operator *(Digit2 lhs, Digit2 rhs) => new Digit2 { mask = lhs.mask & rhs.mask };

        public static explicit operator char(Digit2 digit) =>
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
                _ => throw new InvalidOperationException($"{nameof(Digit2)} is in an invalid state: {digit.mask:X}")
            };

        public static Digit2 Zero = new Digit2 { mask = DigitMask.Zero };
        public static Digit2 One = new Digit2 { mask = DigitMask.One };
        public static Digit2 Two = new Digit2 { mask = DigitMask.Two };
        public static Digit2 Three = new Digit2 { mask = DigitMask.Three };
        public static Digit2 Four = new Digit2 { mask = DigitMask.Four };
        public static Digit2 Five = new Digit2 { mask = DigitMask.Five };
        public static Digit2 Six = new Digit2 { mask = DigitMask.Six };
        public static Digit2 Seven = new Digit2 { mask = DigitMask.Seven };
        public static Digit2 Eight = new Digit2 { mask = DigitMask.Eight };
        public static Digit2 Nine = new Digit2 { mask = DigitMask.Nine }; 
        public static Digit2 All = new Digit2 { mask = DigitMask.All };
    }

    public readonly struct Account
    {
        public string Number { get; init; }

        public static implicit operator string(Account account) => account.Number;

        public override string ToString() => Number;
    }

    public static class AccountHelpers
    {
        public static Account FromDigits(this IEnumerable<Digit2> digits) =>
            new Account
            {
                Number = (new System.Text.StringBuilder()).AppendJoin("", digits.Select(digit => (char) digit)).ToString()
            };
    }
}
