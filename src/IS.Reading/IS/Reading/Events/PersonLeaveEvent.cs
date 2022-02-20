namespace IS.Reading.Events;

public class PersonLeaveEvent : IPersonLeaveEvent
{
    public PersonLeaveEvent(string name, bool isMainCharacter)
        => (PersonName, IsMainCharacter) = (name, isMainCharacter);

    public string PersonName { get; }

    public bool IsMainCharacter { get; }

    public override string ToString()
        => $"person{Helper.MainCharacterSymbol(IsMainCharacter)} leave: {PersonName}";
}