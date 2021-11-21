using System.Collections.Generic;
using System.Linq;

namespace IS.Reading;

public delegate Task AsyncEventHandler<TEventArgs>(object sender, TEventArgs e) where TEventArgs : EventArgs;

public static class AsyncEventHandlerExtensions
{
    public static IEnumerable<AsyncEventHandler<TEventArgs>> GetHandlers<TEventArgs>(
        this AsyncEventHandler<TEventArgs>? handler
    ) where TEventArgs : EventArgs
    {
        if (handler == null)
            return Enumerable.Empty<AsyncEventHandler<TEventArgs>>();

        return handler.GetInvocationList().Cast<AsyncEventHandler<TEventArgs>>();
    }

    public static Task InvokeAllAsync<TEventArgs>(
        this AsyncEventHandler<TEventArgs>? handler,
        object sender,
        TEventArgs e
    ) where TEventArgs : EventArgs
    {
        return Task.WhenAll(handler.GetHandlers().Select(handleAsync => handleAsync(sender, e)));
    }
}

public class EventArgs<T> : EventArgs
{
    public T Data { get; }
    public EventArgs(T data)
    {
        Data = data;
    }

    public override string? ToString() => Data?.ToString();
}
