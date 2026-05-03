namespace IS.Reading.Events;

public class PersonEnterEvent : IPersonEnterEvent
{
    public PersonEnterEvent(string name, bool isMainCharacter)
        => (PersonName, IsMainCharacter) = (name, isMainCharacter);

    public string PersonName { get; }

    public bool IsMainCharacter { get; }

    public override string ToString()
        => $"person{Helper.MainCharacterSymbol(IsMainCharacter)} enter: {PersonName}";
}
