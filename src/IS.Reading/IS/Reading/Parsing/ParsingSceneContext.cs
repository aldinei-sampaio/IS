namespace IS.Reading.Parsing;

public class ParsingSceneContext : IParsingSceneContext
{
    public bool HasMusic { get; set; }
    public bool HasMood { get; set; }
    public void Reset()
    {
        HasMusic = false;
        HasMood = false;
    }
}
