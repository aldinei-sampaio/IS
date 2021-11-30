namespace IS.Reading.Events;

public class BalloonCloseEventTests
{
    [Theory]
    [InlineData(BalloonType.Narration, false, "narration end")]
    [InlineData(BalloonType.Narration, true, "narration end")]
    [InlineData(BalloonType.Tutorial, false, "tutorial end")]
    [InlineData(BalloonType.Tutorial, true, "tutorial end")]
    [InlineData(BalloonType.Speech, false, "speech end")]
    [InlineData(BalloonType.Speech, true, "speech* end")]
    [InlineData(BalloonType.Thought, false, "thought end")]
    [InlineData(BalloonType.Thought, true, "thought* end")]
    public void Initialization(BalloonType balloonType, bool isProtagonist, string description)
    {
        var sut = new BalloonCloseEvent(balloonType, isProtagonist);
        sut.BallonType.Should().Be(balloonType);
        sut.IsProtagonist.Should().Be(isProtagonist);
        sut.ToString().Should().Be(description);
    }
}
