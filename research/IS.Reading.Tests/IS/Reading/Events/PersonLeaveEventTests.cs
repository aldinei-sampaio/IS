namespace IS.Reading.Events;

public class PersonLeaveEventTests
{
    [Theory]
    [InlineData("abc", false, "person leave: abc")]
    [InlineData("DEF", true, "person* leave: DEF")]
    public void Initialization(string personName, bool isMainCharacter, string description)
    {
        var sut = new PersonLeaveEvent(personName, isMainCharacter);
        sut.PersonName.Should().Be(personName);
        sut.IsMainCharacter.Should().Be(isMainCharacter);
        sut.ToString().Should().Be(description);
    }
}
