using IS.Reading.EventObjects;

namespace IS.Reading
{
    public interface IStoryContextEventCaller : IStoryContext
    {
        ISimpleEventCaller Music { get; }
        ISimpleEventCaller Background { get; }
        IOpenCloseEventCaller Narration { get; }
        IOpenCloseEventCaller Tutorial { get; }
        IPersonEventCaller Protagonist { get; }
        IPersonEventCaller Interlocutor { get; }
        IPromptEventCaller<Prompt> Prompt { get; }
        IPromptEventCaller<Display> Display { get; }
        INavigationEventCaller Navigation { get; }
    }
}
