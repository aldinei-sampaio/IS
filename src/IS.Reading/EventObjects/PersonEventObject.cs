using System;

namespace IS.Reading.EventObjects
{

    public class PersonEventObject : SimpleEventObject, IPersonEvents, IPersonEventCaller

    {
        public event EventHandler<string>? OnArrive;
        public event EventHandler? OnLeave;

        public SimpleEventObject Mood { get; } = new SimpleEventObject();
        public OpenCloseEventObject Thought { get; } = new OpenCloseEventObject();
        public OpenCloseEventObject Speech { get; } = new OpenCloseEventObject();
        public PromptEventObject<string> Reward { get; } = new PromptEventObject<string>();

        ISimpleEvents IPersonEvents.Mood => Mood;
        IOpenCloseEvents IPersonEvents.Thought => Thought;
        IOpenCloseEvents IPersonEvents.Speech => Speech;
        IPromptEvents<string> IPersonEvents.Reward => Reward;

        ISimpleEventCaller IPersonEventCaller.Mood => Mood;
        IOpenCloseEventCaller IPersonEventCaller.Thought => Thought;
        IOpenCloseEventCaller IPersonEventCaller.Speech => Speech;
        IPromptEventCaller<string> IPersonEventCaller.Reward => Reward;

        void IPersonEventCaller.Enter(string value) 
            => OnArrive?.Invoke(this, value);

        void IPersonEventCaller.Leave() 
            => OnLeave?.Invoke(this, EventArgs.Empty);
    }
}
