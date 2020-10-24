namespace IS.Reading
{
    public interface IStoryContext
    {
        StringDictionary State { get; }
        IntDictionary Variables { get; }
        string LastChoice { get; set; }
    }
}
