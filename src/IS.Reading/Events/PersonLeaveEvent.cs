namespace IS.Reading.Events;

public class PersonLeaveEvent(string name, bool isMainCharacter) : IPersonLeaveEvent
{
    public string PersonName { get; } = name;

    public bool IsMainCharacter { get; } = isMainCharacter;

    public override string ToString()
        => $"person{Helper.MainCharacterSymbol(IsMainCharacter)} leave: {PersonName}";
}
