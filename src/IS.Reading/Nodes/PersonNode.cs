using IS.Reading.Events;
using IS.Reading.Navigation;

namespace IS.Reading.Nodes;

public class PersonNode(string personName, IBlock childBlock) : INode
{
    public string PersonName { get; } = personName;
    public IBlock? ChildBlock { get; } = childBlock;

    private static bool IsMainCharacter(INavigationContext context, string name)
        => name == context.State.MainCharacterName;

    public async Task<object?> EnterAsync(INavigationContext context)
    {
        var @event = new PersonEnterEvent(PersonName, IsMainCharacter(context, PersonName));
        await context.Events.InvokeAsync<IPersonEnterEvent>(@event);
        context.State.PersonName = PersonName;
        context.State.MoodType = null;
        return null;
    }

    public async Task LeaveAsync(INavigationContext context)
    {
        var @event = new PersonLeaveEvent(PersonName, IsMainCharacter(context, PersonName));
        await context.Events.InvokeAsync<IPersonLeaveEvent>(@event);
        context.State.MoodType = null;
        context.State.PersonName = null;
    }
}
