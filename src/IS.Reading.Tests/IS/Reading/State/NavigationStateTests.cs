namespace IS.Reading.State;

public class NavigationStateTests
{
    [Fact]
    public void Initialization()
    {
        var sut = new NavigationState();
        sut.Background.Should().NotBeNull();
        sut.Background.Should().BeSameAs(BackgroundState.Empty);        
        sut.ProtagonistName.Should().BeNull();
        sut.MoodType.Should().BeNull();
        sut.PersonName.Should().BeNull();
        sut.MusicName.Should().BeNull();

        sut.ProtagonistName = "protagonist";
        sut.MoodType = MoodType.Happy;
        sut.PersonName = "person";
        sut.MusicName = "music";

        sut.ProtagonistName.Should().Be("protagonist");
        sut.MoodType.Should().Be(MoodType.Happy);
        sut.PersonName.Should().Be("person");
        sut.MusicName.Should().Be("music");
    }
}
