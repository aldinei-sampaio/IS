namespace IS.Reading.Parsing;

public class ElementParserSettingsTests
{
    [Fact]
    public void InvalidArgument()
    {
        var ex = Assert.Throws<ArgumentException>(
            () => ElementParserSettings.Normal("gibberish")
        );
        ex.Message.Should().Be("Argumento do tipo 'String' não é válido.");
    }

    [Fact]
    public void Normal()
    {
        var textParser = A.Dummy<ITextParser>();
        var attParser1 = Helper.FakeParser<IAttributeParser>("a");
        var attParser2 = Helper.FakeParser<IAttributeParser>("b");
        var nodeParser1 = Helper.FakeParser<INodeParser>("c");
        var nodeParser2 = Helper.FakeParser<INodeParser>("d");

        var sut = ElementParserSettings.Normal(textParser, attParser1, attParser2, nodeParser1, nodeParser2);
        sut.ShouldBeNormal(textParser, attParser1, attParser2, nodeParser1, nodeParser2);
    }

    [Fact]
    public void NoRepeat()
    {
        var nodeParser1 = Helper.FakeParser<INodeParser>("c");
        var nodeParser2 = Helper.FakeParser<INodeParser>("d");

        var sut = ElementParserSettings.NoRepeat(nodeParser1, nodeParser2);
        sut.ShouldBeNoRepeat(nodeParser1, nodeParser2);
    }

    [Fact]
    public void Aggregated()
    {
        var nodeParser1 = Helper.FakeParser<INodeParser>("c");
        var nodeParser2 = Helper.FakeParser<INodeParser>("d");

        var sut = ElementParserSettings.Aggregated(nodeParser1, nodeParser2);
        sut.ShouldBeAggregated(nodeParser1, nodeParser2);
    }

    [Fact]
    public void OnlyTextParser()
    {
        var parser = A.Dummy<ITextParser>();
        var sut = ElementParserSettings.Normal(parser);
        sut.ShouldBeNormal(parser);
    }

    [Fact]
    public void OnlyAttributeParser()
    {
        var parser = Helper.FakeParser<IAttributeParser>("a");
        var sut = ElementParserSettings.Normal(parser);
        sut.ShouldBeNormal(parser);
    }

    [Fact]
    public void OnlyNodeParser()
    {
        var parser = Helper.FakeParser<INodeParser>("a");
        var sut = ElementParserSettings.Normal(parser);
        sut.ShouldBeNormal(parser);
    }
}
