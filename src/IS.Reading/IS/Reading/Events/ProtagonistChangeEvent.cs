namespace IS.Reading.Events;

public class ProtagonistChangeEvent : IProtagonistChangeEvent
{
    public ProtagonistChangeEvent(string? personName)
        => PersonName = personName;

    public string? PersonName { get; }

    public override string ToString()
    {
        if (PersonName is null)
            return "protagonist undefined";
        else
            return $"protagonist: {PersonName}";
    }
        
}
