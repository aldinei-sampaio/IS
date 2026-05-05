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
    public void Initialization(BalloonType balloonType, bool isMainCharacter, string description)
    {
        var sut = new BalloonOpenEvent(balloonType, isMainCharacter);
        sut.BalloonType.Should().Be(balloonType);
        sut.IsMainCharacter.Should().Be(isMainCharacter);
        sut.ToString().Should().Be(description);
    }
}
