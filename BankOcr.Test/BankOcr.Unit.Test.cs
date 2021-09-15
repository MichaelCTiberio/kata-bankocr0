using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace BankOcr.Tests
{
    public class DigitTests
    {
        [Theory]
        [InlineData(Digit.Zero, '0')]
        [InlineData(Digit.One, '1')]
        [InlineData(Digit.Two, '2')]
        [InlineData(Digit.Three, '3')]
        [InlineData(Digit.Four, '4')]
        [InlineData(Digit.Five, '5')]
        [InlineData(Digit.Six, '6')]
        [InlineData(Digit.Seven, '7')]
        [InlineData(Digit.Eight, '8')]
        [InlineData(Digit.Nine, '9')]
        public void ShouldConvert(string s, char expected)
        {
            Maybe<Digit> hasDigit = Digit.FromString(s);
            Assert.True(hasDigit);

            char actual = hasDigit.Value;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ShouldNotConvert()
        {
            string s = "bad string";

            Maybe<Digit> noDigit = Digit.FromString(s);
            Assert.False(noDigit.HasValue);
        }
    }

    public class FileReaderTests
    {
        [Fact]
        public void ShouldEnumerateLines()
        {
            List<string> expected = new ()
            {
                "This is a bunch",
                "Of multi line text",
                "That we can use",
                "In a test.",
            };

            string text = string.Join("\n", expected);

            StringReader reader = new (text);
            IEnumerable<string> actual = FileReader.Lines(reader).Value;

            Assert.Equal(expected, actual);
        }
    }

    public class UtilityTests
    {
        [Fact]
        public void TypeIs()
        {
            Exception ex = new NullReferenceException();

            var hasEx = ex.MaybeIs<NullReferenceException>();

            Assert.True(hasEx.HasValue);
        }

        [Fact]
        public void TypeIsNot()
        {
            Exception ex = new NullReferenceException();

            var noEx = ex.MaybeIs<InvalidOperationException>();

            Assert.False(noEx.HasValue);
        }

        [Fact]
        public void TryNoException()
        {
            string expected = "no exception";

            Func<Maybe<string>> noThrow = () => expected;
            Func<Exception, bool> handlerThrowsInvalidOperation = (ex) => throw new InvalidOperationException("should not get here");

            Maybe<string> hasString = Utility.Try(
                noThrow,
                Utility.Handler(handlerThrowsInvalidOperation)
            );

            Assert.True(hasString.HasValue);

            string actual = hasString.Value;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TryExceptionCaught()
        {
            Func<Maybe<string>> throwNotImplemented = () => throw new NotImplementedException("should have been caught");
            Func<NotImplementedException, bool> handleNotImplementedException = (ex) => true;

            Maybe<string> noString = Utility.Try(throwNotImplemented, Utility.Handler(handleNotImplementedException));

            Assert.False(noString.HasValue);
        }

        [Fact]
        public void TryExceptionNotCaught()
        {
            Func<Maybe<string>> throwInvalidOperation = () => throw new InvalidOperationException("should not have been caught");
            Func<NotImplementedException, bool> handleNotImplementedException = (ex) => true;

            Action test = () => Utility.Try(throwInvalidOperation, Utility.Handler(handleNotImplementedException));

            Assert.Throws<InvalidOperationException>(test);
        }

        [Fact]
        public void TryExceptionCaughtWithFirstHandler()
        {
            Func<Maybe<string>> throwNotImplemented = () => throw new NotImplementedException("should have been caught");
            Func<NotImplementedException, bool> handleNotImplementedException = (ex) => true;
            Func<Exception, bool> handlerThrowsInvalidOperation = (ex) => throw new InvalidOperationException("should not get here");

            Maybe<string> noString = Utility.Try(
                throwNotImplemented,
                Utility.Handler(handleNotImplementedException),
                Utility.Handler(handlerThrowsInvalidOperation)
            );

            Assert.False(noString.HasValue);
        }


        [Fact]
        public void TryExceptionCaughtWithLastHandler()
        {
            bool runButNotHandled = false;

            Func<Maybe<string>> throwNotImplemented = () => throw new NotImplementedException("should have been caught");
            Func<Exception, bool> doesNotHandleException = (ex) => { runButNotHandled = true; return false; };
            Func<NotImplementedException, bool> handleNotImplementedException = (ex) => true;

            Maybe<string> noString = Utility.Try(
                throwNotImplemented,
                Utility.Handler<NotImplementedException>(doesNotHandleException),
                Utility.Handler<NotImplementedException>(handleNotImplementedException)
            );

            Assert.True(runButNotHandled);
            Assert.False(noString.HasValue);
        }

        private sealed class TestDisposable : IDisposable
        {
            public string TestString { get; init; }
            private readonly Action notifier;

            public TestDisposable(string testString, Action notifier)
            {
                this.TestString = testString;
                this.notifier = notifier;
            }

            private bool disposedValue = false;
            void IDisposable.Dispose()
            {
                if (!disposedValue)
                {
                    notifier();
                    disposedValue = true;
                }
                GC.SuppressFinalize(this);
            }
        }

        [Fact]
        public void UseShouldOpenAndDisposeObject()
        {
            string expected = "test string";
            bool isDisposed = false;

            var hasTestString = Utility.Use(
                new TestDisposable(expected, () => isDisposed = true),
                (resource) => resource.TestString
            );

            Assert.True(isDisposed);
            Assert.True(hasTestString.HasValue);
            string actual = hasTestString.Value;
            Assert.Equal(expected, actual);
        }
    }

    public class ProgramTests
    {
        [Fact]
        public void FilenameFromArgsShouldSucceed()
        {
            string expected = "file path";
            Func<IndexOutOfRangeException, bool> handler =
                Utility.Handler<IndexOutOfRangeException>(
                    (ex) => throw new InvalidOperationException("Could not find file name."));

            var hasFilename = Program.FilenameFromArgs(new [] { expected }, handler);

            Assert.True(hasFilename.HasValue);
            string actual = hasFilename.Value;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void FilenameFromArgsShouldFail()
        {
            Func<IndexOutOfRangeException, bool> noHandler = (ex) => false;

            Assert.Throws<IndexOutOfRangeException>(() => Program.FilenameFromArgs(new string[] { }, noHandler));
        }

        [Theory]
        [InlineData("000000000")]
        [InlineData("111111111")]
        [InlineData("222222222")]
        [InlineData("333333333")]
        [InlineData("444444444")]
        [InlineData("555555555")]
        [InlineData("666666666")]
        [InlineData("777777777")]
        [InlineData("888888888")]
        [InlineData("999999999")]
        [InlineData("123456789")]
        public void ShouldGetAccountNumbersFromTextStream(string expected)
        {
            var lines = TestLib.AccountLinesFromAccountNumber(expected).Value;
            var maybeAccounts = Program.AccountNumbersFromTextLines(lines);

            Assert.True(maybeAccounts.HasValue);
            string actual = maybeAccounts.Value.First().Number;
            Assert.Equal(expected, actual);
        }
    }

    public static class TestLib
    {
        private static bool MaybeHasValue<T>(Maybe<T> maybe) => maybe.HasValue;

        private static Maybe<IEnumerable<T>> MaybeEnumerable<T>(this IEnumerable<Maybe<T>> maybes)
        {
            return maybes.All(MaybeHasValue) ?
                Maybe<IEnumerable<T>>.Wrap(EnumerableFromEnumerableMaybe(maybes)) :
                Maybe<IEnumerable<T>>.None;

            // The following function assumes that all the Maybe<T> items have a value.
            // That is dangerous in general, so we hide it in the method scope.
            static IEnumerable<T> EnumerableFromEnumerableMaybe(IEnumerable<Maybe<T>> maybes)
            {
                foreach (var item in maybes)
                    yield return item.Value;
            }
        }

        private static Maybe<IEnumerable<Digit>> FromAccountNumber(string accountNumber) =>
            accountNumber.AsEnumerable().Select(Digit.FromChar).MaybeEnumerable();

        public static Maybe<IEnumerable<string>> AccountLinesFromAccountNumber(string accountNumber)
        {
            LinkedList<string> top = new ();
            LinkedList<string> middle = new ();
            LinkedList<string> bottom = new ();

            var maybeDigits = FromAccountNumber(accountNumber);

            if (!maybeDigits) return Maybe<IEnumerable<string>>.None;

            foreach (var d in maybeDigits.Value)
            {
                top.AddLast(d.Top);
                middle.AddLast(d.Middle);
                bottom.AddLast(d.Bottom);
            }

            string stop = (new StringBuilder()).AppendJoin("+", top).ToString();
            string smiddle = (new StringBuilder()).AppendJoin("+", middle).ToString();
            string sbottom = (new StringBuilder()).AppendJoin("+", bottom).ToString();

            return new []
            {
                stop,
                smiddle,
                sbottom,
                new string(' ', (accountNumber.Length - 1))
            };
        }
    }
}
