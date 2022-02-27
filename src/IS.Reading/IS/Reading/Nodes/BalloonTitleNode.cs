using IS.Reading.Events;
using IS.Reading.Navigation;
using IS.Reading.Variables;

namespace IS.Reading.Nodes;

public class BalloonTitleNode : INode
{
    public BalloonTitleNode(ITextSource? textSource)
        => TextSource = textSource;

    public ITextSource? TextSource { get; }

    private static async Task<object?> ApplyStateAsync(INavigationContext context, string? newValue)
    {
        var oldValue = context.State.Title;

        if (oldValue == newValue)
            return oldValue;

        if (newValue is not null)
        {
            var @event = new BalloonTitleEvent(newValue);
            await context.Events.InvokeAsync<IBalloonTitleEvent>(@event);
        }

        context.State.Title = newValue;

        return oldValue;
    }

    public Task<object?> EnterAsync(INavigationContext context)
        => ApplyStateAsync(context, TextSource?.Build(context.Variables));

    public Task EnterAsync(INavigationContext context, object? state)
        => ApplyStateAsync(context, state as string);
}