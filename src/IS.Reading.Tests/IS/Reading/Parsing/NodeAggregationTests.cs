namespace IS.Reading.Parsing;

public class NodeAggregationTests
{
    [Fact]
    public void NoArguments()
    {
        var sut = new NodeAggregation();
        sut.ChildParsers.Count.Should().Be(0);
    }

    [Fact]
    public void NullArgument()
    {
        var ex = Assert.Throws<ArgumentNullException>(
            () => new NodeAggregation(null)
        );
        ex.ParamName.Should().Be("nodeParsers");
    }

    [Fact]
    public void OneArgument()
    {
        var parser = Helper.FakeNodeParser("a");
        var sut = new NodeAggregation(parser);
        sut.ChildParsers.Count.Should().Be(1);
        sut.ChildParsers["a"].Should().Be(parser);
    }

    [Fact]
    public void TwoArguments()
    {
        var aParser = Helper.FakeNodeParser("a");
        var bParser = Helper.FakeNodeParser("b");
        var sut = new NodeAggregation(aParser, bParser);
        sut.ChildParsers.Count.Should().Be(2);
        sut.ChildParsers["a"].Should().Be(aParser);
        sut.ChildParsers["b"].Should().Be(bParser);
    }
}
