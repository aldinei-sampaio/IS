using IS.Reading.Conditions;
using IS.Reading.Events;
using IS.Reading.Navigation;

namespace IS.Reading.Nodes;

public class MusicNode : INode
{
    public string? MusicName { get; }

    public MusicNode(string? musicName, ICondition? when)
        => (MusicName, When) = (musicName, when);

    public ICondition? When { get; }

    public async Task<INode> EnterAsync(INavigationContext context)
    {
        var oldName = context.State.MusicName;
        if (oldName == MusicName)
            return this;

        await context.Events.InvokeAsync<IMusicChangeEvent>(new MusicChangeEvent(MusicName));
        context.State.MusicName = MusicName;

        return new MusicNode(oldName, When);
    }
}
