﻿namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public class SpeechChildNodeParserTests
{
    [Fact]
    public void Initialization()
    {
        var elementParser = A.Dummy<IElementParser>();
        var speechTextNodeParser = Helper.FakeParser<ISpeechTextNodeParser>("speech");
        A.CallTo(() => speechTextNodeParser.BalloonType).Returns(BalloonType.Speech);
        var choiceNodeParser = Helper.FakeParser<IChoiceNodeParser>("choice");

        var sut = new SpeechChildNodeParser(elementParser, speechTextNodeParser, choiceNodeParser);
        sut.Name.Should().Be("speech");
        sut.BalloonType.Should().Be(BalloonType.Speech);
        sut.Settings.ShouldBeAggregatedNonRepeat(speechTextNodeParser);
        sut.AggregationSettings.ShouldBeAggregatedNonRepeat(choiceNodeParser);
    }
}
