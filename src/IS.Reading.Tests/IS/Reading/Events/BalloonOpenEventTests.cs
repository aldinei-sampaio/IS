namespace IS.Reading.Events;

public class BalloonOpenEventTests
{
    [Theory]
    [InlineData(BalloonType.Narration, false, "narration start")]
    [InlineData(BalloonType.Narration, true, "narration start")]
    [InlineData(BalloonType.Tutorial, false, "tutorial start")]
    [InlineData(BalloonType.Tutorial, true, "tutorial start")]
    [InlineData(BalloonType.Speech, false, "speech start")]
    [InlineData(BalloonType.Speech, true, "speech* start")]
    [InlineData(BalloonType.Thought, false, "thought start")]
    [InlineData(BalloonType.Thought, true, "thought* start")]
    public void Initialization(BalloonType balloonType, bool isProtagonist, string description)
    {
        var sut = new BalloonOpenEvent(balloonType, isProtagonist);
        sut.BallonType.Should().Be(balloonType);
        sut.IsProtagonist.Should().Be(isProtagonist);
        sut.ToString().Should().Be(description);
    }
}
