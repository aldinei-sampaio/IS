namespace IS.Reading.Parsing;

public interface IParsingSceneContext
{
    bool HasMusic { get; set; }
    bool HasMood { get; set; }
    void Reset();
}