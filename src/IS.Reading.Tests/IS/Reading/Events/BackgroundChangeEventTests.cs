using IS.Reading.State;

namespace IS.Reading.Events;

public class BackgroundChangeEventTests
{
    [Fact]
    public void Initialization()
    {
        var toStringText = "Texto retornado pelo método ToString() do BackgroundState";

        var state = A.Dummy<IBackgroundState>();
        A.CallTo(() => state.ToString()).Returns(toStringText);

        var sut = new BackgroundChangeEvent(state);
        sut.State.Should().BeSameAs(state);
        sut.ToString().Should().BeSameAs(toStringText);
    }
}
