namespace IS.Reading.Events;

public class MusicChangeEventTests
{
    [Theory]
    [InlineData("abc", "music: abc")]
    [InlineData(null, "music undefined")]
    public void Initialization(string musicName, string description)
    {
        var sut = new MusicChangeEvent(musicName);
        sut.MusicName.Should().Be(musicName);
        sut.ToString().Should().Be(description);
    }
}
