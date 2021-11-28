using IS.Reading.Events;
using IS.Reading.Navigation;
using IS.Reading.State;

namespace IS.Reading.Nodes;

public class ProtagonistChangeNode : INode
{
    public string? Name { get; }

    public ProtagonistChangeNode(string? name, ICondition? when)
        => (Name, When) = (name, when);

    public ICondition? When { get; }

    public async Task<INode> EnterAsync(INavigationContext context)
    {
        var oldName = context.State.Protagonist;
        if (oldName == Name)
            return this;

        await context.Events.InvokeAsync<IProtagonistChangeEvent>(new ProtagonistChangeEvent(Name));
        context.State.Protagonist = Name;

        return new ProtagonistChangeNode(oldName, When);
    }
}
