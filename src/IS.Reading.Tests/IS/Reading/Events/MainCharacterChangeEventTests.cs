namespace IS.Reading.Events;

public class MainCharacterChangeEventTests
{
    [Theory]
    [InlineData("abc", "mc: abc")]
    [InlineData(null, "mc unset")]
    public void Initialization(string personName, string description)
    {
        var sut = new MainCharacterChangeEvent(personName);
        sut.PersonName.Should().Be(personName);
        sut.ToString().Should().Be(description);
    }
}
