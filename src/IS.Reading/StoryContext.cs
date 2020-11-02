using IS.Reading.EventObjects;
using System.Collections.Generic;

namespace IS.Reading
{
    public class StoryContext : IStoryContextEventCaller, IStoryContextEvents
    {
        public StringDictionary State { get; } = new StringDictionary();
        public IntDictionary Variables { get; } = new IntDictionary();
        public StringDictionary Names { get; } = new StringDictionary();
        public string LastChoice { get; set; } = string.Empty;

        public SimpleEventObject Music { get; } = new SimpleEventObject();
        public SimpleEventObject Background { get; } = new SimpleEventObject();
        public OpenCloseEventObject Narration { get; } = new OpenCloseEventObject();
        public OpenCloseEventObject Tutorial { get; } = new OpenCloseEventObject();
        public PersonEventObject Protagonist { get; } = new PersonEventObject();
        public PersonEventObject Interlocutor { get; } = new PersonEventObject();
        public PromptEventObject<Prompt> Prompt { get; } = new PromptEventObject<Prompt>();
        public PromptEventObject<Display> Display { get; } = new PromptEventObject<Display>();
        public NavigationEventObject Navigation { get; } = new NavigationEventObject();

        ISimpleEvents IStoryContextEvents.Music => Music;
        ISimpleEvents IStoryContextEvents.Background => Background;
        IOpenCloseEvents IStoryContextEvents.Narration => Narration;
        IOpenCloseEvents IStoryContextEvents.Tutorial => Tutorial;
        IPersonEvents IStoryContextEvents.Protagonist => Protagonist;
        IPersonEvents IStoryContextEvents.Interlocutor => Interlocutor;
        IPromptEvents<Prompt> IStoryContextEvents.Prompt => Prompt;
        IPromptEvents<Display> IStoryContextEvents.Display => Display;
        INavigationEvents IStoryContextEvents.Navigation => Navigation;

        ISimpleEventCaller IStoryContextEventCaller.Music => Music;
        ISimpleEventCaller IStoryContextEventCaller.Background => Background;
        IOpenCloseEventCaller IStoryContextEventCaller.Narration => Narration;
        IOpenCloseEventCaller IStoryContextEventCaller.Tutorial => Tutorial;
        IPersonEventCaller IStoryContextEventCaller.Protagonist => Protagonist;
        IPersonEventCaller IStoryContextEventCaller.Interlocutor => Interlocutor;
        IPromptEventCaller<Prompt> IStoryContextEventCaller.Prompt => Prompt;
        IPromptEventCaller<Display> IStoryContextEventCaller.Display => Display;
        INavigationEventCaller IStoryContextEventCaller.Navigation => Navigation;
    }
}
