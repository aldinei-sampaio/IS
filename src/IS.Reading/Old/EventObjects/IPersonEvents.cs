namespace IS.Reading.EventObjects;

public interface IPersonEvents : ISimpleEvents
{
    event AsyncEventHandler<EventArgs<string>>? OnArriveAsync;
    event AsyncEventHandler<EventArgs>? OnLeaveAsync;
    event AsyncEventHandler<EventArgs>? OnBumpAsync;

    ISimpleEvents Mood { get; }
    IOpenCloseEvents Thought { get; }
    IOpenCloseEvents Speech { get; }
    IPromptEvents<VarIncrement> Reward { get; }
}
