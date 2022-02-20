using IS.Reading.Choices;
using IS.Reading.Events;
using IS.Reading.Navigation;
using IS.Reading.Variables;

namespace IS.Reading.Nodes;

public class BalloonTextNode : IPauseNode
{
    public ITextSource TextSource { get; }
    public BalloonType BalloonType { get; }
    public IChoiceBuilder? ChoiceBuilder { get; }

    public BalloonTextNode(ITextSource textSource, BalloonType ballonType, IChoiceBuilder? choiceBuilder)
        => (TextSource, BalloonType, ChoiceBuilder) = (textSource, ballonType, choiceBuilder);

    public async Task<object?> EnterAsync(INavigationContext context)
    {
        var text = TextSource.Build(context.Variables);
        var choice = ChoiceBuilder?.Build(context);
        var @event = new BalloonTextEvent(text, BalloonType, context.State.IsMainCharacter(), choice);
        await context.Events.InvokeAsync<IBalloonTextEvent>(@event);
        return null;
    }
}
