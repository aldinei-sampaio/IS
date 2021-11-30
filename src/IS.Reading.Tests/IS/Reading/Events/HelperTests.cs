namespace IS.Reading.Events;

public class HelperTests
{
    [Theory]
    [InlineData(BalloonType.Narration, true, "")]
    [InlineData(BalloonType.Narration, false, "")]
    [InlineData(BalloonType.Tutorial, true, "")]
    [InlineData(BalloonType.Tutorial, false, "")]
    [InlineData(BalloonType.Speech, true, "*")]
    [InlineData(BalloonType.Speech, false, "")]
    [InlineData(BalloonType.Thought, true, "*")]
    [InlineData(BalloonType.Thought, false, "")]
    public void ProtagSymbol1(BalloonType balloonType, bool isProtagonist, string expected)
    {
        Helper.ProtagSymbol(balloonType, isProtagonist).Should().Be(expected);
    }

    [Theory]
    [InlineData(true, "*")]
    [InlineData(false, "")]
    public void ProtagSymbol2(bool isProtagonist, string expected)
    {
        Helper.ProtagSymbol(isProtagonist).Should().Be(expected);
    }
}
