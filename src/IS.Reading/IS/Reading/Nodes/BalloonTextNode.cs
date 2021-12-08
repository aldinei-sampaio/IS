using IS.Reading.Choices;
using IS.Reading.Events;
using IS.Reading.Navigation;

namespace IS.Reading.Nodes;

public class BalloonTextNode : IPauseNode
{
    public string Text { get; }
    public BalloonType BalloonType { get; }
    public IChoiceNode? ChoiceNode { get; }

    public BalloonTextNode(string text, BalloonType ballonType, IChoiceNode? choiceNode)
        => (Text, BalloonType, ChoiceNode) = (text, ballonType, choiceNode);

    public async Task<INode> EnterAsync(INavigationContext context)
    {
        var choice = CreateChoice(ChoiceNode, context);
        var @event = new BalloonTextEvent(Text, BalloonType, context.State.IsProtagonist(), choice);
        await context.Events.InvokeAsync<IBalloonTextEvent>(@event);
        return this;
    }

    private static IChoice? CreateChoice(IChoiceNode? choiceNode, INavigationContext context)
    {
        if (choiceNode is null)
            return null;

        var options = new List<IChoiceOption>();
        foreach(var option in choiceNode.Options)
        {
            if (option.VisibleWhen is not null && !option.VisibleWhen.Evaluate(context.Variables))
                continue;

            var isEnabled = option.EnabledWhen is null || option.EnabledWhen.Evaluate(context.Variables);
            var text = isEnabled ? option.Text : option.DisabledText ?? option.Text;

            options.Add(new ChoiceOption(option.Key, text, isEnabled, option.ImageName, option.HelpText));
        }

        if (options.Count == 0 || options.All(i => !i.IsEnabled))
            return null;

        return new Choice(options, choiceNode.TimeLimit, choiceNode.Default);
    }
}
