﻿using IS.Reading.Choices;

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
    public void Initialization(string text, BalloonType balloonType, bool isMainCharacter, string description)
    {
        var sut = new BalloonTextEvent(text, balloonType, isMainCharacter, null);
        sut.Text.Should().Be(text);
        sut.BalloonType.Should().Be(balloonType);
        sut.IsMainCharacter.Should().Be(isMainCharacter);
        sut.ToString().Should().Be(description);
        sut.Choice.Should().BeNull();
    }

    [Fact]
    public void InitializateChoices()
    {
        var choice = A.Dummy<IChoice>();
        var sut = new BalloonTextEvent("teste", BalloonType.Speech, true, choice);
        sut.Choice.Should().BeSameAs(choice);
    }
}
