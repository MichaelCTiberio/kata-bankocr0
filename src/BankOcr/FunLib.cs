using System;
using System.Collections.Generic;
using System.Linq;

namespace FunLib;

public readonly struct Maybe<T>
{
    private readonly T? _value;

    public T Value
    {
        readonly get
        {
            if (!hasValue || _value == null)
                throw new InvalidOperationException($"{nameof(Maybe<T>)} does not contain a value");

            return _value;
        }

        init { _value = value; }
    }

    public static explicit operator T(Maybe<T> maybe) => maybe.Value;

    private readonly bool hasValue;
    public static implicit operator bool(Maybe<T> maybe) => maybe.hasValue;

    private Maybe(T? value, bool hasValue)
    {
        this._value = value;
        this.hasValue = hasValue;
    }

    public static implicit operator Maybe<T>(T value) => Maybe<T>.Wrap(value);
    public static Maybe<T> Wrap(T value) => new (value: value, hasValue: (value != null));

    public static Maybe<T> None { get; } = new (value: default, hasValue: false);

    public readonly bool HasValue() => hasValue;
    public static bool HasValue(Maybe<T> maybe) => maybe.HasValue();

    public readonly Maybe<TReturn> Map<TReturn>(Func<T, TReturn> func) =>
        hasValue ?
            func(Value) :
            Maybe<TReturn>.None;

    public readonly Maybe<TReturn> Bind<TReturn>(Func<T, Maybe<TReturn>> func) =>
        hasValue ?
            func(Value) :
            Maybe<TReturn>.None;

    public readonly Maybe<T> HaveAction(Action<T> action)
    {
        if (hasValue)
            action(Value);

        return this;
    }

    public readonly Maybe<T> HaveAction(Action action)
    {
        if (hasValue)
            action();

        return this;
    }

    public readonly Maybe<T> EmptyAction(Action action)
    {
        if (!hasValue)
            action();

        return this;
    }

    public override string? ToString()
    {
        return (hasValue) ?
            Value?.ToString() :
            "<empty>";
    }
}

public static class MaybeHelpers
{
    public static Maybe<IEnumerable<T>> MaybeEnumerable<T>(this IEnumerable<Maybe<T>> maybes)
    {
        return maybes.All(Maybe<T>.HasValue) ?
            Maybe<IEnumerable<T>>.Wrap(EnumerableFromEnumerableMaybe(maybes)) :
            Maybe<IEnumerable<T>>.None;

        // The following function assumes that all the Maybe<T> items have a value.
        static IEnumerable<T> EnumerableFromEnumerableMaybe(IEnumerable<Maybe<T>> maybes)
        {
            foreach (var item in maybes)
                yield return item.Value;
        }
    }

    public static Maybe<T> Next<T>(this IEnumerator<T> en) =>
        en.MoveNext() ?
            en.Current :
            Maybe<T>.None;
}

public static class Fn
{
    public static Maybe<T> MaybeTypeIs<T>(this object obj)
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
            var maybeException = exception.MaybeTypeIs<TException>();
            return maybeException ?
                handler((TException) maybeException) :
                false;
        };

    public static Func<Exception, bool> Handler<TException>(Func<bool> isHandled)
        where TException : Exception =>
        (Exception exception) =>
            exception.MaybeTypeIs<TException>() ?
                isHandled() :
                false;

    public static Func<Exception, bool> Handler<TException>(Action handler)
        where TException : Exception =>
        Handler<TException>(() => { handler(); return true; });

    public static T Use<TDisposable, T>(TDisposable disposable, Func<TDisposable, T> func)
        where TDisposable : IDisposable
    {
        using TDisposable resource = disposable;
        return func(resource);
    }
}
