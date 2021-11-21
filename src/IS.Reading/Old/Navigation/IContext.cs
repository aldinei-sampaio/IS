namespace IS.Reading.Navigation
{
    public interface IContext
    {
        StringDictionary State { get; }
        IVariableDictionary Variables { get; }
    }
}
