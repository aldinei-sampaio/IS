namespace IS.Reading.Events;

public class MoodChangeEvent : IMoodChangeEvent
{
    public MoodChangeEvent(string personName, MoodType emotionType)
        => (PersonName, MoodType) = (personName, emotionType);

    public MoodType MoodType { get; }

    public string PersonName { get; }

    public override string ToString()
        => $"mood: {MoodType.ToString().ToLower()}";
}
