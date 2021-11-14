namespace IS.Reading.EventObjects;

public class PersonEventObject : SimpleEventObject, IPersonEvents, IPersonEventCaller

{
    public event AsyncEventHandler<EventArgs<string>>? OnArriveAsync;
    public event AsyncEventHandler<EventArgs>? OnLeaveAsync;
    public event AsyncEventHandler<EventArgs>? OnBumpAsync;

    public SimpleEventObject Mood { get; } = new SimpleEventObject();
    public OpenCloseEventObject Thought { get; } = new OpenCloseEventObject();
    public OpenCloseEventObject Speech { get; } = new OpenCloseEventObject();
    public PromptEventObject<VarIncrement> Reward { get; } = new PromptEventObject<VarIncrement>();

    ISimpleEvents IPersonEvents.Mood => Mood;
    IOpenCloseEvents IPersonEvents.Thought => Thought;
    IOpenCloseEvents IPersonEvents.Speech => Speech;
    IPromptEvents<VarIncrement> IPersonEvents.Reward => Reward;

    ISimpleEventCaller IPersonEventCaller.Mood => Mood;
    IOpenCloseEventCaller IPersonEventCaller.Thought => Thought;
    IOpenCloseEventCaller IPersonEventCaller.Speech => Speech;
    IPromptEventCaller<VarIncrement> IPersonEventCaller.Reward => Reward;

    Task IPersonEventCaller.EnterAsync(string value)
        => OnArriveAsync.InvokeAllAsync(this, new(value));

    Task IPersonEventCaller.LeaveAsync()
        => OnLeaveAsync.InvokeAllAsync(this, EventArgs.Empty);

    Task IPersonEventCaller.BumpAsync()
        => OnBumpAsync.InvokeAllAsync(this, EventArgs.Empty);
}
