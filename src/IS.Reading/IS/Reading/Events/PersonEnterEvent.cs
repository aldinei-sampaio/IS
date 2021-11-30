namespace IS.Reading.Events;

public class PersonEnterEvent : IPersonEnterEvent
{
    public PersonEnterEvent(string name, bool isProtagonist)
        => (PersonName, IsProtagonist) = (name, isProtagonist);

    public string PersonName { get; }

    public bool IsProtagonist { get; }

    public override string ToString()
        => $"person enter: {PersonName}{Helper.ProtagSymbol(IsProtagonist)}";
}
