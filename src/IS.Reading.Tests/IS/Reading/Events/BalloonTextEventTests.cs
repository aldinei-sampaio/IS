namespace IS.Reading.Events;

public class BalloonTextEventTests
{
    [Theory]
    [InlineData("abc", BalloonType.Narration, false, "narration: abc")]
    [InlineData("DEF", BalloonType.Narration, true, "narration: DEF")]
    [InlineData("ghi", BalloonType.Tutorial, false, "tutorial: ghi")]
    [InlineData("jkl", BalloonType.Tutorial, true, "tutorial: jkl")]
    [InlineData("Mno", BalloonType.Speech, false, "speech: Mno")]
    [InlineData("pqR", BalloonType.Speech, true, "speech*: pqR")]
    [InlineData("sTu", BalloonType.Thought, false, "thought: sTu")]
    [InlineData("wvx", BalloonType.Thought, true, "thought*: wvx")]
    public void Initialization(string text, BalloonType balloonType, bool isProtagonist, string description)
    {
        var sut = new BalloonTextEvent(text, balloonType, isProtagonist);
        sut.Text.Should().Be(text);
        sut.BalloonType.Should().Be(balloonType);
        sut.IsProtagonist.Should().Be(isProtagonist);
        sut.ToString().Should().Be(description);
    }
}
