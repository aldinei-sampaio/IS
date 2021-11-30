namespace IS.Reading.State;

public class NavigationStateTests
{
    [Fact]
    public void Initialization()
    {
        var sut = new NavigationState();
        sut.Background.Should().NotBeNull();
        sut.Background.Should().BeSameAs(BackgroundState.Empty);        
        sut.Protagonist.Should().BeNull();
        sut.MoodType.Should().BeNull();
        sut.PersonName.Should().BeNull();

        sut.Protagonist = "protagonist";
        sut.MoodType = MoodType.Happy;
        sut.PersonName = "person";

        sut.Protagonist.Should().Be("protagonist");
        sut.MoodType.Should().Be(MoodType.Happy);
        sut.PersonName.Should().Be("person");
    }
}
