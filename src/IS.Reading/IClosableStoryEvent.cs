namespace IS.Reading
{
    public interface IClosableStoryEvent : IStoryEvent
    {
        void Close(IStoryContextUpdater context);
    }
}
