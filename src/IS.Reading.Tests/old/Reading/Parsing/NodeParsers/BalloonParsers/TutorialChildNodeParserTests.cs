using IS.Reading.Variables;

namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public class TutorialChildNodeParserTests
{
    [Fact]
    public void Initialization()
    {
        var elementParser = A.Dummy<IElementParser>();
        var tutorialTextNodeParser = Helper.FakeParser<ITutorialTextNodeParser>("tutorial");
        A.CallTo(() => tutorialTextNodeParser.BalloonType).Returns(BalloonType.Tutorial);
        var choiceNodeParser = Helper.FakeParser<IChoiceNodeParser>("choice");
        var textSourceParser = A.Dummy<ITextSourceParser>();

        var sut = new TutorialChildNodeParser(elementParser, textSourceParser, tutorialTextNodeParser, choiceNodeParser);
        sut.Name.Should().Be("tutorial");
        sut.BalloonType.Should().Be(BalloonType.Tutorial);
        sut.Settings.ShouldBeAggregatedNonRepeat(tutorialTextNodeParser);
        sut.AggregationSettings.ShouldBeAggregatedNonRepeat(choiceNodeParser);
    }
}
