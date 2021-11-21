namespace IS.Reading.EventObjects;

public interface IPersonEventCaller : ISimpleEventCaller
{
    Task EnterAsync(string value);
    Task LeaveAsync();
    Task BumpAsync();
    ISimpleEventCaller Mood { get; }
    IOpenCloseEventCaller Thought { get; }
    IOpenCloseEventCaller Speech { get; }
    IPromptEventCaller<VarIncrement> Reward { get; }
}
