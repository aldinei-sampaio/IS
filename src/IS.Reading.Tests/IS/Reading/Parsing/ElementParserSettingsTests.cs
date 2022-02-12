namespace IS.Reading.Parsing;

public class ElementParserSettingsTests
{
    [Fact]
    public void NoBlock()
    {
        var nodeParser1 = Helper.FakeParser<INodeParser>("c");
        var nodeParser2 = Helper.FakeParser<INodeParser>("d");

        var sut = new ElementParserSettings.NoBlock(nodeParser1, nodeParser2);
        sut.ShouldContainOnly(nodeParser1, nodeParser2);
        sut.Should().BeEquivalentTo(new
        {
            IsBlock = false,
            NoRepeatNode = false,
            ExitOnUnknownNode = false,
            ExitOnElse = false
        });
    }

    [Fact]
    public void Block()
    {
        var nodeParser1 = Helper.FakeParser<INodeParser>("c");
        var nodeParser2 = Helper.FakeParser<INodeParser>("d");

        var sut = new ElementParserSettings.Block(nodeParser1, nodeParser2);
        sut.ShouldContainOnly(nodeParser1, nodeParser2);
        sut.Should().BeEquivalentTo(new
        {
            IsBlock = true,
            NoRepeatNode = false,
            ExitOnUnknownNode = false,
            ExitOnElse = false
        });
    }

    [Fact]
    public void IfBlock()
    {
        var nodeParser1 = Helper.FakeParser<INodeParser>("c");
        var nodeParser2 = Helper.FakeParser<INodeParser>("d");

        var sut = new ElementParserSettings.IfBlock(nodeParser1, nodeParser2);
        sut.ShouldContainOnly(nodeParser1, nodeParser2);
        sut.Should().BeEquivalentTo(new
        {
            IsBlock = true,
            NoRepeatNode = false,
            ExitOnUnknownNode = false,
            ExitOnElse = true
        });
    }

    [Fact]
    public void AggregatedNonRepeat()
    {
        var nodeParser1 = Helper.FakeParser<INodeParser>("c");
        var nodeParser2 = Helper.FakeParser<INodeParser>("d");

        var sut = new ElementParserSettings.AggregatedNonRepeat(nodeParser1, nodeParser2);
        sut.ShouldContainOnly(nodeParser1, nodeParser2);
        sut.Should().BeEquivalentTo(new
        {
            IsBlock = false,
            NoRepeatNode = true,
            ExitOnUnknownNode = true,
            ExitOnElse = false
        });
    }

    [Fact]
    public void Aggregated()
    {
        var nodeParser1 = Helper.FakeParser<INodeParser>("c");
        var nodeParser2 = Helper.FakeParser<INodeParser>("d");

        var sut = new ElementParserSettings.Aggregated(nodeParser1, nodeParser2);
        sut.ShouldContainOnly(nodeParser1, nodeParser2);
        sut.Should().BeEquivalentTo(new
        {
            IsBlock = false,
            NoRepeatNode = false,
            ExitOnUnknownNode = true,
            ExitOnElse = false
        });
    }
}
