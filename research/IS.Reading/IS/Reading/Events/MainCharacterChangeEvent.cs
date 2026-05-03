namespace IS.Reading.Events;

public class MainCharacterChangeEvent : IMainCharacterChangeEvent
{
    public MainCharacterChangeEvent(string? personName)
        => PersonName = personName;

    public string? PersonName { get; }

    public override string ToString()
    {
        if (PersonName is null)
            return "mc unset";
        else
            return $"mc: {PersonName}";
    }        
}
