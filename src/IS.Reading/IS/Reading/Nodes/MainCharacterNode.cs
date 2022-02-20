using IS.Reading.Events;
using IS.Reading.Navigation;

namespace IS.Reading.Nodes;

public class MainCharacterNode : INode
{
    public string? MainCharacterName { get; }

    public MainCharacterNode(string? personName)
        => MainCharacterName = personName;

    public static async Task<object?> ApplyStateAsync(INavigationContext context, string? newName)
    {
        var oldName = context.State.MainCharacterName;
        if (oldName == newName)
            return oldName;

        await context.Events.InvokeAsync<IMainCharacterChangeEvent>(new MainCharacterChangeEvent(newName));
        context.State.MainCharacterName = newName;

        return oldName;
    }

    public Task<object?> EnterAsync(INavigationContext context)
        => ApplyStateAsync(context, MainCharacterName);

    public Task EnterAsync(INavigationContext context, object? state)
        => ApplyStateAsync(context, state as string);
}
