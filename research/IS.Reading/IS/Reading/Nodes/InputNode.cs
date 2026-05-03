using IS.Reading.Events;
using IS.Reading.Navigation;

namespace IS.Reading.Nodes;

public class InputNode : IPauseNode
{
    public IInputBuilder InputBuilder { get; }

    public InputNode(IInputBuilder inputBuilder)
        => InputBuilder = inputBuilder;

    public async Task<object?> EnterAsync(INavigationContext context)
    {
        var @event = InputBuilder.BuildEvent(context.Variables);     
        context.State.WaitingFor = InputBuilder.Key;        
        var oldValue = context.Variables[InputBuilder.Key];
        await context.Events.InvokeAsync<IInputEvent>(@event);
        return oldValue;
    }

    public async Task EnterAsync(INavigationContext context, object? state)
    {
        await EnterAsync(context);
        context.Variables[InputBuilder.Key] = state;
    }
}