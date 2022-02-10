namespace IS.Reading.Events;

public class ProtagonistChangeEventTests
{
    [Theory]
    [InlineData("abc", "protagonist: abc")]
    [InlineData(null, "protagonist undefined")]
    public void Initialization(string personName, string description)
    {
        var sut = new ProtagonistChangeEvent(personName);
        sut.PersonName.Should().Be(personName);
        sut.ToString().Should().Be(description);
    }
}
