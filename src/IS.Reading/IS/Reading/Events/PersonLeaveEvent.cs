namespace IS.Reading.Events;

public class PersonLeaveEvent : IPersonLeaveEvent
{
    public PersonLeaveEvent(string name, bool isProtagonist)
        => (PersonName, IsProtagonist) = (name, isProtagonist);

    public string PersonName { get; }

    public bool IsProtagonist { get; }

    public override string ToString()
        => $"person leave: {PersonName}{Helper.ProtagSymbol(IsProtagonist)}";
}