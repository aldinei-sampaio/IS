namespace IS.Reading.Events;

public class StoryContextEventObject : IStoryContextEvents, IStoryContextEventCaller
{
    public SimpleEventObject Music { get; } = new();
    public SimpleEventObject Background { get; } = new();
    public OpenCloseEventObject Narration { get; } = new();
    public OpenCloseEventObject Tutorial { get; } = new();
    public PersonEventObject Protagonist { get; } = new();
    public PersonEventObject Interlocutor { get; } = new();
    public PromptEventObject<Prompt> Prompt { get; } = new();
    public PromptEventObject<Display> Display { get; } = new();
    public NavigationEventObject Navigation { get; } = new();
    public PromptEventObject<Trophy> Trophy { get; } = new();

    ISimpleEvents IStoryContextEvents.Music => Music;
    ISimpleEvents IStoryContextEvents.Background => Background;
    IOpenCloseEvents IStoryContextEvents.Narration => Narration;
    IOpenCloseEvents IStoryContextEvents.Tutorial => Tutorial;
    IPersonEvents IStoryContextEvents.Protagonist => Protagonist;
    IPersonEvents IStoryContextEvents.Interlocutor => Interlocutor;
    IPromptEvents<Prompt> IStoryContextEvents.Prompt => Prompt;
    IPromptEvents<Display> IStoryContextEvents.Display => Display;
    INavigationEvents IStoryContextEvents.Navigation => Navigation;
    IPromptEvents<Trophy> IStoryContextEvents.Trophy => Trophy;

    ISimpleEventCaller IStoryContextEventCaller.Music => Music;
    ISimpleEventCaller IStoryContextEventCaller.Background => Background;
    IOpenCloseEventCaller IStoryContextEventCaller.Narration => Narration;
    IOpenCloseEventCaller IStoryContextEventCaller.Tutorial => Tutorial;
    IPersonEventCaller IStoryContextEventCaller.Protagonist => Protagonist;
    IPersonEventCaller IStoryContextEventCaller.Interlocutor => Interlocutor;
    IPromptEventCaller<Prompt> IStoryContextEventCaller.Prompt => Prompt;
    IPromptEventCaller<Display> IStoryContextEventCaller.Display => Display;
    INavigationEventCaller IStoryContextEventCaller.Navigation => Navigation;
    IPromptEventCaller<Trophy> IStoryContextEventCaller.Trophy => Trophy;
}
