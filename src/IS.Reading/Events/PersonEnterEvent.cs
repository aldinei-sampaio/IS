namespace IS.Reading.Events;

public class PersonEnterEvent(string name, bool isMainCharacter) : IPersonEnterEvent
{
    public string PersonName { get; } = name;

    public bool IsMainCharacter { get; } = isMainCharacter;

    public override string ToString()
        => $"person{Helper.MainCharacterSymbol(IsMainCharacter)} enter: {PersonName}";
}
