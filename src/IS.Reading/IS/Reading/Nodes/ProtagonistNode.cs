using IS.Reading.Events;
using IS.Reading.Navigation;

namespace IS.Reading.Nodes;

public class ProtagonistNode : INode
{
    public string? ProtagonistName { get; }

    public ProtagonistNode(string? personName, ICondition? when)
        => (ProtagonistName, When) = (personName, when);

    public ICondition? When { get; }

    public async Task<INode> EnterAsync(INavigationContext context)
    {
        var oldName = context.State.ProtagonistName;
        if (oldName == ProtagonistName)
            return this;

        await context.Events.InvokeAsync<IProtagonistChangeEvent>(new ProtagonistChangeEvent(ProtagonistName));
        context.State.ProtagonistName = ProtagonistName;

        return new ProtagonistNode(oldName, When);
    }
}
