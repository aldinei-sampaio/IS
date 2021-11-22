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

        var attParser1 = A.Fake<IAttributeParser>();
        A.CallTo(() => attParser1.ElementName).Returns("a");
        var attParser2 = A.Fake<IAttributeParser>();
        A.CallTo(() => attParser2.ElementName).Returns("b");
        var nodeParser1 = A.Fake<INodeParser>();
        A.CallTo(() => nodeParser1.ElementName).Returns("c");
        var nodeParser2 = A.Dummy<INodeParser>();
        A.CallTo(() => nodeParser2.ElementName).Returns("d");

        var sut = new ElementParserSettings(textParser, attParser1, attParser2, nodeParser1, nodeParser2);

        sut.TextParser.Should().BeSameAs(textParser);
        sut.AttributeParsers.Should().HaveCount(2);
        sut.AttributeParsers["a"].Should().BeSameAs(attParser1);
        sut.AttributeParsers["b"].Should().BeSameAs(attParser2);
        sut.ChildParsers.Should().HaveCount(2);
        sut.ChildParsers["c"].Should().BeSameAs(nodeParser1);
        sut.ChildParsers["d"].Should().BeSameAs(nodeParser2);
    }

    [Fact]
    public void OnlyTextParser()
    {
        var textParser = A.Dummy<ITextParser>();
        var sut = new ElementParserSettings(textParser);

        sut.TextParser.Should().BeSameAs(textParser);
        sut.AttributeParsers.Should().BeEmpty();
        sut.ChildParsers.Should().BeEmpty();
    }

    [Fact]
    public void OnlyAttributeParser()
    {
        var attParser = A.Fake<IAttributeParser>();
        A.CallTo(() => attParser.ElementName).Returns("a");
        var sut = new ElementParserSettings(attParser);

        sut.TextParser.Should().BeNull();
        sut.AttributeParsers.Should().HaveCount(1);
        sut.AttributeParsers["a"].Should().BeSameAs(attParser);
        sut.ChildParsers.Should().BeEmpty();
    }

    [Fact]
    public void OnlyNodeParser()
    {
        var attParser = A.Fake<INodeParser>();
        A.CallTo(() => attParser.ElementName).Returns("a");
        var sut = new ElementParserSettings(attParser);

        sut.TextParser.Should().BeNull();
        sut.AttributeParsers.Should().BeEmpty();
        sut.ChildParsers.Should().HaveCount(1);
        sut.ChildParsers["a"].Should().BeSameAs(attParser);
    }
}
