namespace IS.Reading
{
    public interface ICondition
    {
        bool Evaluate(IStoryContextUpdater context);
    }
}
