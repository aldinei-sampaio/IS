namespace IS.Reading.Events;

public interface IPersonEventCaller : ISimpleEventCaller
{
    Task EnterAsync(string value);
    Task LeaveAsync();
    Task BumpAsync();
    ISimpleEventCaller Mood { get; }
    IOpenCloseEventCaller Thought { get; }
    IOpenCloseEventCaller Speech { get; }
    IPromptEventCaller<Reward> Reward { get; }
}
