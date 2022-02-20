namespace IS.Reading.Parsing;

public class ElementParserSettingsTests
{
    [Fact]
    public void NoBlock()
    {
        var nodeParser1 = Helper.FakeParser<INodeParser>("c");
        var nodeParser2 = Helper.FakeParser<INodeParser>("d");

        var sut = new ElementParserSettings.NoBlock(nodeParser1, nodeParser2);
        sut.ChildParsers.Should().BeEquivalentTo(new[] { nodeParser1, nodeParser2 });
        sut.Should().BeEquivalentTo(new
        {
            NoRepeatNode = false,
            ExitOnEnd = false,
            ExitOnUnknownNode = false,
            ExitOnElse = false,
            ParseCurrent = false
        });
    }

    [Fact]
    public void Block()
    {
        var nodeParser1 = Helper.FakeParser<INodeParser>("c");
        var nodeParser2 = Helper.FakeParser<INodeParser>("d");

        var sut = new ElementParserSettings.Block(nodeParser1, nodeParser2);
        sut.ChildParsers.Should().BeEquivalentTo(new[] { nodeParser1, nodeParser2 });
        sut.Should().BeEquivalentTo(new
        {
            NoRepeatNode = false,
            ExitOnEnd = true,
            ExitOnUnknownNode = false,
            ExitOnElse = false,
            ParseCurrent = false
        });
    }

    [Fact]
    public void IfBlock()
    {
        var nodeParser1 = Helper.FakeParser<INodeParser>("c");
        var nodeParser2 = Helper.FakeParser<INodeParser>("d");

        var sut = new ElementParserSettings.IfBlock(nodeParser1, nodeParser2);
        sut.ChildParsers.Should().BeEquivalentTo(new[] { nodeParser1, nodeParser2 });
        sut.Should().BeEquivalentTo(new
        {
            NoRepeatNode = false,
            ExitOnEnd = true,
            ExitOnUnknownNode = false,
            ExitOnElse = true,
            ParseCurrent = false
        });
    }

    [Fact]
    public void AggregatedNonRepeat()
    {
        var nodeParser1 = Helper.FakeParser<INodeParser>("c");
        var nodeParser2 = Helper.FakeParser<INodeParser>("d");

        var sut = new ElementParserSettings.AggregatedNonRepeat(nodeParser1, nodeParser2);
        sut.ChildParsers.Should().BeEquivalentTo(new[] { nodeParser1, nodeParser2 });
        sut.Should().BeEquivalentTo(new
        {
            NoRepeatNode = true,
            ExitOnEnd = false,
            ExitOnUnknownNode = true,
            ExitOnElse = false,
            ParseCurrent = false
        });
    }

    [Fact]
    public void Aggregated()
    {
        var nodeParser1 = Helper.FakeParser<INodeParser>("c");
        var nodeParser2 = Helper.FakeParser<INodeParser>("d");

        var sut = new ElementParserSettings.Aggregated(nodeParser1, nodeParser2);
        sut.ChildParsers.Should().BeEquivalentTo(new[] { nodeParser1, nodeParser2 });
        sut.Should().BeEquivalentTo(new
        {
            NoRepeatNode = false,
            ExitOnEnd = false,
            ExitOnUnknownNode = true,
            ExitOnElse = false,
            ParseCurrent = false
        });
    }

    [Fact]
    public void AggregatedCurrent()
    {
        var nodeParser1 = Helper.FakeParser<INodeParser>("c");
        var nodeParser2 = Helper.FakeParser<INodeParser>("d");
        var parsers = new[] { nodeParser1, nodeParser2 };

        var sut = new ElementParserSettings.AggregatedCurrent(parsers);
        sut.ChildParsers.Should().BeEquivalentTo(parsers);
        sut.Should().BeEquivalentTo(new
        {
            NoRepeatNode = false,
            ExitOnEnd = false,
            ExitOnUnknownNode = true,
            ExitOnElse = false,
            ParseCurrent = true
        });
    }
}
