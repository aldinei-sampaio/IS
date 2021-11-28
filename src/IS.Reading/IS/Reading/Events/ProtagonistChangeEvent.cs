namespace IS.Reading.Events;

public class ProtagonistChangeEvent : IProtagonistChangeEvent
{
    public ProtagonistChangeEvent(string? name)
        => Name = name;

    public string? Name { get; }

    public override string ToString()
    {
        if (Name is null)
            return "protagonist undefined";
        else
            return $"protagonist: {Name}";
    }
        
}
