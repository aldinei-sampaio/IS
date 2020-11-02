using System;

namespace IS.Reading.EventObjects
{
    public interface IPersonEvents : ISimpleEvents
    {
        event EventHandler<string>? OnArrive;
        event EventHandler? OnLeave;
        event EventHandler? OnBump;

        ISimpleEvents Mood { get; }
        IOpenCloseEvents Thought { get; }
        IOpenCloseEvents Speech { get; }
        IPromptEvents<VarIncrement> Reward { get; }
    }
}
