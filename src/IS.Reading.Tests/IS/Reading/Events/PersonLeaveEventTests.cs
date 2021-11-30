namespace IS.Reading.Events;

public class PersonLeaveEventTests
{
    [Theory]
    [InlineData("abc", false, "person leave: abc")]
    [InlineData("DEF", true, "person leave: DEF*")]
    public void Initialization(string personName, bool isProtagonist, string description)
    {
        var sut = new PersonLeaveEvent(personName, isProtagonist);
        sut.PersonName.Should().Be(personName);
        sut.IsProtagonist.Should().Be(isProtagonist);
        sut.ToString().Should().Be(description);
    }
}
