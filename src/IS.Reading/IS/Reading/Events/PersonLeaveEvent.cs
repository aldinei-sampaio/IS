namespace IS.Reading.Events;

public class PersonLeaveEvent : IPersonLeaveEvent
{
    public PersonLeaveEvent(string name, bool isProtagonist)
        => (Name, IsProtagonist) = (name, IsProtagonist);

    public string Name { get; }

    public bool IsProtagonist { get; }

    public override string ToString()
        => $"person{(IsProtagonist ? "*" : string.Empty)} leave: {Name}";
}