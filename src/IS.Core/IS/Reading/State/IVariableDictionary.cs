namespace IS.Reading.State
{
    public interface IVariableDictionary
    {
        int this[string name] { get; set; }
        int Count { get; }
    }
}
