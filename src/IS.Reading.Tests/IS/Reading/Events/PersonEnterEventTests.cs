namespace IS.Reading.Events;

public class PersonEnterEventTests
{
    [Theory]
    [InlineData("abc", false, "person enter: abc")]
    [InlineData("DEF", true, "person enter: DEF*")]
    public void Initialization(string personName, bool isProtagonist, string description)
    {
        var sut = new PersonEnterEvent(personName, isProtagonist);
        sut.PersonName.Should().Be(personName);
        sut.IsProtagonist.Should().Be(isProtagonist);
        sut.ToString().Should().Be(description);
    }
}
