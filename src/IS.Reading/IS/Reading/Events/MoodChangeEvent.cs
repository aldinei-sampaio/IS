namespace IS.Reading.Events;

public class MoodChangeEvent : IMoodChangeEvent
{
    public MoodChangeEvent(string personName, bool isProtagonist, MoodType emotionType)
        => (PersonName, IsProtagonist, MoodType) = (personName, isProtagonist, emotionType);

    public MoodType MoodType { get; }

    public string PersonName { get; }

    public bool IsProtagonist { get; }

    public override string ToString()
        => $"mood{Helper.ProtagSymbol(IsProtagonist)} {MoodType.ToString().ToLower()}: {PersonName}";
}
