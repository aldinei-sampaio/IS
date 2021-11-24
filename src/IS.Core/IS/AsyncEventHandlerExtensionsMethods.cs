namespace IS.Reading;

public static class AsyncEventHandlerExtensionsMethods
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
