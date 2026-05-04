namespace IS.Reading.Events;

public class MainCharacterChangeEvent(string? personName) : IMainCharacterChangeEvent
{
    public string? PersonName { get; } = personName;

    public override string ToString()
    {
        if (PersonName is null)
            return "mc unset";
        else
            return $"mc: {PersonName}";
    }
}
