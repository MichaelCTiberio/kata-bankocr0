using System;
using Xunit;

namespace FunLib.Tests
{
    public class FnTests
    {
        [Fact]
        public void TypeIs()
        {
            Exception ex = new NullReferenceException();

            var hasEx = ex.MaybeTypeIs<NullReferenceException>();

            Assert.True(hasEx.HasValue());
        }

        [Fact]
        public void TypeIsNot()
        {
            Exception ex = new NullReferenceException();

            var noEx = ex.MaybeTypeIs<InvalidOperationException>();

            Assert.False(noEx.HasValue());
        }

        [Fact]
        public void TryNoException()
        {
            string expected = "no exception";

            Func<string> noThrow = () => expected;
            Func<Exception, bool> handlerThrowsInvalidOperation = (ex) => throw new InvalidOperationException("should not get here");

            Maybe<string> hasString = Fn.Try(
                noThrow,
                Fn.Handler(handlerThrowsInvalidOperation)
            );

            Assert.True(hasString.HasValue());

            string actual = hasString.Value;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TryExceptionCaught()
        {
            Func<string> throwNotImplemented = () => throw new NotImplementedException("should have been caught");
            Func<NotImplementedException, bool> handleNotImplementedException = (ex) => true;

            Maybe<string> noString = Fn.Try(throwNotImplemented, Fn.Handler(handleNotImplementedException));

            Assert.False(noString.HasValue());
        }

        [Fact]
        public void TryExceptionNotCaught()
        {
            Func<Maybe<string>> throwInvalidOperation = () => throw new InvalidOperationException("should not have been caught");
            Func<NotImplementedException, bool> handleNotImplementedException = (ex) => true;

            Action test = () => Fn.Try(throwInvalidOperation, Fn.Handler(handleNotImplementedException));

            Assert.Throws<InvalidOperationException>(test);
        }

        [Fact]
        public void TryExceptionCaughtWithFirstHandler()
        {
            Func<string> throwNotImplemented = () => throw new NotImplementedException("should have been caught");
            Func<NotImplementedException, bool> handleNotImplementedException = (ex) => true;
            Func<Exception, bool> handlerThrowsInvalidOperation = (ex) => throw new InvalidOperationException("should not get here");

            Maybe<string> noString = Fn.Try(
                throwNotImplemented,
                Fn.Handler(handleNotImplementedException),
                Fn.Handler(handlerThrowsInvalidOperation)
            );

            Assert.False(noString.HasValue());
        }


        [Fact]
        public void TryExceptionCaughtWithLastHandler()
        {
            bool runButNotHandled = false;

            Func<string> throwNotImplemented = () => throw new NotImplementedException("should have been caught");
            Func<Exception, bool> doesNotHandleException = (ex) => { runButNotHandled = true; return false; };
            Func<NotImplementedException, bool> handleNotImplementedException = (ex) => true;

            Maybe<string> noString = Fn.Try(
                throwNotImplemented,
                Fn.Handler<NotImplementedException>(doesNotHandleException),
                Fn.Handler<NotImplementedException>(handleNotImplementedException)
            );

            Assert.True(runButNotHandled);
            Assert.False(noString.HasValue());
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

            var actual = Fn.Use(
                new TestDisposable(expected, () => isDisposed = true),
                (resource) => resource.TestString
            );

            Assert.True(isDisposed);
            Assert.Equal(expected, actual);
        }
    }
}
