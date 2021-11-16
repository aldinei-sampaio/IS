namespace IS.Reading.Navigation
{
    public interface INavigationContext
    {
        StringDictionary State { get; }
        IVariableDictionary Variables { get; }
    }
}
