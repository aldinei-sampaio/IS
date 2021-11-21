using IS.Reading.EventObjects;

namespace IS.Reading
{
    public interface IStoryContextEvents : IStoryContext
    {
        ISimpleEvents Music { get; }
        ISimpleEvents Background { get; }
        IOpenCloseEvents Narration { get; }
        IOpenCloseEvents Tutorial { get; }
        IPersonEvents Protagonist { get; }
        IPersonEvents Interlocutor { get; }
        IPromptEvents<Prompt> Prompt { get; }
        IPromptEvents<Display> Display { get; }
        INavigationEvents Navigation { get; }
        IPromptEvents<Trophy> Trophy { get; }
    }
}
