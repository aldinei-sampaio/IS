namespace IS.Reading
{
    public interface IStoryEvent
    {
        IStoryEvent? Execute(IStoryContextUpdater context);
    }
}
