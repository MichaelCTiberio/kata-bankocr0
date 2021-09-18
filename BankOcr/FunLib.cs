using System;
using System.Collections.Generic;
using System.Linq;

namespace FunLib
{
    public readonly struct Maybe<T>
    {
        public T Value { readonly get; init; }
        public static explicit operator T(Maybe<T> maybe) => maybe.Value;

        private readonly bool hasValue;
        public static implicit operator bool(Maybe<T> maybe) => maybe.hasValue;

        private Maybe(T value, bool hasValue)
        {
            this.Value = value;
            this.hasValue = hasValue;
        }

        public static implicit operator Maybe<T>(T value) => Maybe<T>.Wrap(value);
        public static Maybe<T> Wrap(T value) => new (value: value, hasValue: (value != null));

        public static Maybe<T> None { get; } = new (value: default, hasValue: false);

        public readonly bool HasValue() => hasValue;
        public static bool HasValue(Maybe<T> maybe) => maybe.HasValue();

        public readonly Maybe<TReturn> Map<TReturn>(Func<T, TReturn> f) =>
            hasValue ?
                f(Value) :
                Maybe<TReturn>.None;

        public override string ToString() =>
            (hasValue) ? Value.ToString() : "<empty>";
    }

    public static class MaybeHelpers
    {
        public static Maybe<IEnumerable<T>> MaybeEnumerable<T>(this IEnumerable<Maybe<T>> maybes)
        {
            return maybes.All(Maybe<T>.HasValue) ?
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

    }

    public static class Fn
    {
        public static Maybe<T> MaybeIs<T>(this object obj)
        {
            if (obj is T output)
                return output;

            return Maybe<T>.None;
        }

        public static Maybe<T> Try<T>(Func<T> f, IEnumerable<Func<Exception, bool>> handlers)
        {
            try
            {
                 return f();
            }
            catch (Exception ex)
            {
                if (!handlers.Any((handler) => handler(ex)))
                    throw;
            }

            return Maybe<T>.None;
        }

        public static Maybe<T> Try<T>(Func<T> func, params Func<Exception, bool> [] handlers) =>
            Try(func, (IEnumerable<Func<Exception, bool>>) handlers);

        public static Func<Exception, bool> Handler<TException>(Func<TException, bool> handler)
            where TException : Exception =>
            (Exception exception) =>
            {
                var maybeException = exception.MaybeIs<TException>();
                return maybeException ?
                    handler((TException) maybeException) :
                    false;
            };

        public static T Use<TDisposable, T>(TDisposable disposable, Func<TDisposable, T> func)
            where TDisposable : IDisposable
        {
            using TDisposable resource = disposable;
            return func(resource);
        }
    }
}
