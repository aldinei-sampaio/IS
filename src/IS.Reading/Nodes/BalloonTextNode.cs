using IS.Reading.Choices;
using IS.Reading.Events;
using IS.Reading.Navigation;
using IS.Reading.Variables;

namespace IS.Reading.Nodes;

public class BalloonTextNode(ITextSource textSource, BalloonType balloonType, IChoiceBuilder? choiceBuilder) : IPauseNode
{
    public ITextSource TextSource { get; } = textSource;
    public BalloonType BalloonType { get; } = balloonType;
    public IChoiceBuilder? ChoiceBuilder { get; } = choiceBuilder;

    public async Task<object?> EnterAsync(INavigationContext context)
    {
        var text = TextSource.Build(context.Variables);
        var choice = ChoiceBuilder?.Build(context);
        if (choice is not null)
            context.State.WaitingFor = choice.Key;
        var @event = new BalloonTextEvent(text, BalloonType, context.State.IsMainCharacter(), choice);
        await context.Events.InvokeAsync<IBalloonTextEvent>(@event);
        return null;
    }
}
