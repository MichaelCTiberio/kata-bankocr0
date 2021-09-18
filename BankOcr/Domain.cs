using System;
using System.Collections.Generic;

namespace BankOcr.Domain
{
    public readonly struct Digit
    {
        private readonly char value;
        public static implicit operator char(Digit digit) => digit.value;

        private Digit(char value) => this.value = value;

        public static Digit FromChar(char c) =>
            c switch
            {
                >= '0' and <= '9' => new Digit(c),
                _ => throw new NotImplementedException("Invalid char"),
            };

        public static Digit FromString(string s) =>
            s switch
            {
                Zero => new Digit('0'),
                One => new Digit('1'),
                Two => new Digit('2'),
                Three => new Digit('3'),
                Four => new Digit('4'),
                Five => new Digit('5'),
                Six => new Digit('6'),
                Seven => new Digit('7'),
                Eight => new Digit('8'),
                Nine => new Digit('9'),
                _ => throw new NotImplementedException("Invalid string"),
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

        public static string Top(Digit d) =>
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

        public static string Middle(Digit d) =>
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

        public static string Bottom(Digit d) =>
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

        public override string ToString() => value.ToString();
    }

    public readonly struct Account
    {
        public string Number { get; init; }

        public static implicit operator string(Account account) => account.Number;

        public static Account FromDigits(IEnumerable<Digit> digits)
        {
            using var endigits = digits.GetEnumerator();

            endigits.MoveNext();
            char digit1 = endigits.Current;
            endigits.MoveNext();
            char digit2 = endigits.Current;
            endigits.MoveNext();
            char digit3 = endigits.Current;
            endigits.MoveNext();
            char digit4 = endigits.Current;
            endigits.MoveNext();
            char digit5 = endigits.Current;
            endigits.MoveNext();
            char digit6 = endigits.Current;
            endigits.MoveNext();
            char digit7 = endigits.Current;
            endigits.MoveNext();
            char digit8 = endigits.Current;
            endigits.MoveNext();
            char digit9 = endigits.Current;

            string number = $"{digit1}{digit2}{digit3}" +
                            $"{digit4}{digit5}{digit6}" +
                            $"{digit7}{digit8}{digit9}";

            return new Account { Number = number };
        }

        public override string ToString() => Number;
    }
}
