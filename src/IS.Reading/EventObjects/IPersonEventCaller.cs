namespace IS.Reading.EventObjects
{
    public interface IPersonEventCaller : ISimpleEventCaller
    {
        void Enter(string value);
        void Leave();
        void Bump();
        ISimpleEventCaller Mood { get; }
        IOpenCloseEventCaller Thought { get; }
        IOpenCloseEventCaller Speech { get; }
        IPromptEventCaller<VarIncrement> Reward { get; }
    }
}
