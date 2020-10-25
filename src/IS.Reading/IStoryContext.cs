namespace IS.Reading
{
    public interface IStoryContext
    {
        StringDictionary State { get; }
        IntDictionary Variables { get; }
        StringDictionary Names { get; }
        string LastChoice { get; set; }
    }
}
