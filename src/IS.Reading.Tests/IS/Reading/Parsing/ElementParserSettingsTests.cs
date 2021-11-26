namespace IS.Reading.Parsing;

public class ElementParserSettingsTests
{
    [Fact]
    public void InvalidArgument()
    {
        var ex = Assert.Throws<ArgumentException>(
            () => new ElementParserSettings("gibberish")
        );
        ex.Message.Should().Be("Argumento do tipo 'String' não é válido.");
    }

    [Fact]
    public void MultipleParsers()
    {
        var textParser = A.Dummy<ITextParser>();

        var attParser1 = Helper.FakeParser<IAttributeParser>("a");
        var attParser2 = Helper.FakeParser<IAttributeParser>("b");
        var nodeParser1 = Helper.FakeParser<INodeParser>("c");
        var nodeParser2 = Helper.FakeParser<INodeParser>("d");

        var sut = new ElementParserSettings(textParser, attParser1, attParser2, nodeParser1, nodeParser2);

        sut.TextParser.Should().BeSameAs(textParser);
        sut.AttributeParsers.Count.Should().Be(2);
        sut.AttributeParsers["a"].Should().BeSameAs(attParser1);
        sut.AttributeParsers["b"].Should().BeSameAs(attParser2);
        sut.ChildParsers.Count.Should().Be(2);
        sut.ChildParsers["c"].Should().BeSameAs(nodeParser1);
        sut.ChildParsers["d"].Should().BeSameAs(nodeParser2);
    }

    [Fact]
    public void OnlyTextParser()
    {
        var textParser = A.Dummy<ITextParser>();
        var sut = new ElementParserSettings(textParser);

        sut.TextParser.Should().BeSameAs(textParser);
        sut.AttributeParsers.Count.Should().Be(0);
        sut.ChildParsers.Count.Should().Be(0);
    }

    [Fact]
    public void OnlyAttributeParser()
    {
        var attParser = Helper.FakeParser<IAttributeParser>("a");
        var sut = new ElementParserSettings(attParser);

        sut.TextParser.Should().BeNull();
        sut.AttributeParsers.Count.Should().Be(1);
        sut.AttributeParsers["a"].Should().BeSameAs(attParser);
        sut.ChildParsers.Count.Should().Be(0);
    }

    [Fact]
    public void OnlyNodeParser()
    {
        var attParser = Helper.FakeParser<INodeParser>("a");
        var sut = new ElementParserSettings(attParser);

        sut.TextParser.Should().BeNull();
        sut.AttributeParsers.Count.Should().Be(0);
        sut.ChildParsers.Count.Should().Be(1);
        sut.ChildParsers["a"].Should().BeSameAs(attParser);
    }
}
