using System;
using System.Collections.Generic;
using System.IO;
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
            string expected = "result string";
            bool isDisposed = false;
            bool didRun = false;

            var hasString = Utility.Use(
                new TestDisposable(expected, () => isDisposed = true),
                (resource) => { didRun = true; return resource.TestString; }
            );

            Assert.True(didRun);
            Assert.True(isDisposed);
            Assert.True(hasString.HasValue);
            string actual = hasString.Value;
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
    }
}
