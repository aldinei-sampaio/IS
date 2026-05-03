using IS.Reading.Choices;

namespace IS.Reading.Parsing.NodeParsers;

public class BalloonChildParsingContextTests
{
    [Fact]
    public void Initialization()
    {
        var balloonType = BalloonType.Speech;
        var sut = new BalloonChildParsingContext(balloonType);
        sut.BalloonType.Should().Be(balloonType);
        sut.ChoiceBuilder.Should().BeNull();
        sut.Nodes.Should().BeEmpty();
    }

    [Fact]
    public void ReadWriteProperties()
    {
        var choiceBuilder = A.Dummy<IChoiceBuilder>();
        var sut = new BalloonChildParsingContext(BalloonType.Narration);
        sut.ChoiceBuilder = choiceBuilder;
        sut.ChoiceBuilder.Should().BeSameAs(choiceBuilder);
    }
}
