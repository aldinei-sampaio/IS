﻿using IS.Reading.Variables;

namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public class NarrationChildNodeParserTests
{
    [Fact]
    public void Initialization()
    {
        var elementParser = A.Dummy<IElementParser>();
        var narrationTextNodeParser = Helper.FakeParser<INarrationTextNodeParser>("narration");
        A.CallTo(() => narrationTextNodeParser.BalloonType).Returns(BalloonType.Narration);
        var choiceNodeParser = Helper.FakeParser<IChoiceNodeParser>("choice");
        var textSourceParser = A.Dummy<ITextSourceParser>();

        var sut = new NarrationChildNodeParser(elementParser, textSourceParser, narrationTextNodeParser, choiceNodeParser);
        sut.Name.Should().Be("narration");
        sut.BalloonType.Should().Be(BalloonType.Narration);
        sut.Settings.ShouldBeAggregatedNonRepeat(narrationTextNodeParser);
        sut.AggregationSettings.ShouldBeAggregatedNonRepeat(choiceNodeParser);
    }
}
