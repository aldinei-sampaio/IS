namespace IS.Reading.Events;

public class PersonEnterEvent : IPersonEnterEvent
{
    public PersonEnterEvent(string name, bool isProtagonist)
        => (Name, IsProtagonist) = (name, IsProtagonist);

    public string Name { get; }

    public bool IsProtagonist { get; }

    public override string ToString()
        => $"person{Helper.ProtagSymbol(IsProtagonist)} enter: {Name}";
}
