namespace IS.Reading.Events;

public class MoodChangeEvent(string personName, bool isMainCharacter, MoodType moodType) : IMoodChangeEvent
{
    public MoodType MoodType { get; } = moodType;

    public string PersonName { get; } = personName;

    public bool IsMainCharacter { get; } = isMainCharacter;

    public override string ToString()
        => $"mood{Helper.MainCharacterSymbol(IsMainCharacter)} {MoodType.ToString().ToLower()}: {PersonName}";
}
