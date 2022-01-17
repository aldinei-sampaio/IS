using IS.Reading.Conditions;
using IS.Reading.Events;
using IS.Reading.Navigation;

namespace IS.Reading.Nodes;

public class ProtagonistNode : INode
{
    public string? ProtagonistName { get; }

    public ProtagonistNode(string? personName, ICondition? when)
        => (ProtagonistName, When) = (personName, when);

    public ICondition? When { get; }

    public static async Task<object?> ApplyStateAsync(INavigationContext context, string? protagonistName)
    {
        var oldName = context.State.ProtagonistName;
        if (oldName == protagonistName)
            return null;

        await context.Events.InvokeAsync<IProtagonistChangeEvent>(new ProtagonistChangeEvent(protagonistName));
        context.State.ProtagonistName = protagonistName;

        return oldName;
    }

    public Task<object?> EnterAsync(INavigationContext context)
        => ApplyStateAsync(context, ProtagonistName);

    public Task EnterAsync(INavigationContext context, object? state)
        => ApplyStateAsync(context, state as string);
}
