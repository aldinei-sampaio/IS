namespace IS.Reading.Events;

public interface IPersonEvents : ISimpleEvents
{
    event AsyncEventHandler<EventArgs<string>>? OnArriveAsync;
    event AsyncEventHandler<EventArgs>? OnLeaveAsync;
    event AsyncEventHandler<EventArgs>? OnBumpAsync;

    ISimpleEvents Mood { get; }
    IOpenCloseEvents Thought { get; }
    IOpenCloseEvents Speech { get; }
    IPromptEvents<Reward> Reward { get; }
}
