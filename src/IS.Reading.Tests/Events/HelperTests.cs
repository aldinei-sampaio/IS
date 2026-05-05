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
    public void ProtagSymbol1(BalloonType balloonType, bool isMainCharacter, string expected)
    {
        Helper.MainCharacterSymbol(balloonType, isMainCharacter).Should().Be(expected);
    }

    [Theory]
    [InlineData(true, "*")]
    [InlineData(false, "")]
    public void ProtagSymbol2(bool isMainCharacter, string expected)
    {
        Helper.MainCharacterSymbol(isMainCharacter).Should().Be(expected);
    }
}
