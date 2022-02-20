namespace IS.Reading.Events;

public class PersonEnterEventTests
{
    [Theory]
    [InlineData("abc", false, "person enter: abc")]
    [InlineData("DEF", true, "person* enter: DEF")]
    public void Initialization(string personName, bool isMainCharacter, string description)
    {
        var sut = new PersonEnterEvent(personName, isMainCharacter);
        sut.PersonName.Should().Be(personName);
        sut.IsMainCharacter.Should().Be(isMainCharacter);
        sut.ToString().Should().Be(description);
    }
}
