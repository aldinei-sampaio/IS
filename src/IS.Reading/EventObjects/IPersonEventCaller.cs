namespace IS.Reading.EventObjects
{
    public interface IPersonEventCaller : ISimpleEventCaller
    {
        void Enter(string value);
        void Leave();
        ISimpleEventCaller Mood { get; }
        IOpenCloseEventCaller Thought { get; }
        IOpenCloseEventCaller Speech { get; }
        IPromptEventCaller<string> Reward { get; }
    }
}
