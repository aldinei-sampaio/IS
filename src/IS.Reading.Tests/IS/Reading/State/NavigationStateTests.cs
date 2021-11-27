namespace IS.Reading.State;

public class NavigationStateTests
{
    [Fact]
    public void Initialization()
    {
        var sut = new NavigationState();
        sut.Background.Should().NotBeNull();
        sut.Background.Should().BeSameAs(BackgroundState.Empty);
    }
}
