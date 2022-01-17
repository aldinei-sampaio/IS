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

    public static async Task<object?> ApplyStateAsync(INavigationContext context, string? musicName)
    {
        var oldName = context.State.MusicName;
        if (oldName == musicName)
            return null;

        await context.Events.InvokeAsync<IMusicChangeEvent>(new MusicChangeEvent(musicName));
        context.State.MusicName = musicName;

        return oldName;
    }

    public Task<object?> EnterAsync(INavigationContext context)
        => ApplyStateAsync(context, MusicName);

    public Task EnterAsync(INavigationContext context, object? state)
        => ApplyStateAsync(context, state as string);
}
