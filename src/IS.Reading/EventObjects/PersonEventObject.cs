using System;

namespace IS.Reading.EventObjects
{

    public class PersonEventObject : SimpleEventObject, IPersonEvents, IPersonEventCaller

    {
        public event EventHandler<string>? OnArrive;
        public event EventHandler? OnLeave;
        public event EventHandler? OnBump;

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

        void IPersonEventCaller.Enter(string value) 
            => OnArrive?.Invoke(this, value);

        void IPersonEventCaller.Leave() 
            => OnLeave?.Invoke(this, EventArgs.Empty);

        void IPersonEventCaller.Bump()
            => OnBump?.Invoke(this, EventArgs.Empty);
    }
}
