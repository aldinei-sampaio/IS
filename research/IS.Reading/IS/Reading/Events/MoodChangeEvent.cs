namespace IS.Reading.Events;

public class MoodChangeEvent : IMoodChangeEvent
{
    public MoodChangeEvent(string personName, bool isMainCharacter, MoodType emotionType)
        => (PersonName, IsMainCharacter, MoodType) = (personName, isMainCharacter, emotionType);

    public MoodType MoodType { get; }

    public string PersonName { get; }

    public bool IsMainCharacter { get; }

    public override string ToString()
        => $"mood{Helper.MainCharacterSymbol(IsMainCharacter)} {MoodType.ToString().ToLower()}: {PersonName}";
}
