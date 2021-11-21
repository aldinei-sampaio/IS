using IS.Reading.Navigation;
using IS.Reading.Parsing.Attributes;
using System.Xml;

namespace IS.Reading.Parsing;

public class ElementParserTests
{
    [Fact]
    public async Task Empty()
    {
        using var textReader = new StringReader("<teste />");
        using var reader = XmlReader.Create(textReader, new() { Async = true });
        reader.MoveToContent();

        var context = A.Dummy<IParsingContext>();

        var sut = new ElementParser();

        var parsed = await sut.ParseAsync(reader, context);

        parsed.Should().NotBeNull();
        parsed.Text.Should().BeNull();
        parsed.Block.Should().BeNull();
        parsed.When.Should().BeNull();
        parsed.While.Should().BeNull();
    }

    [Theory]
    [InlineData("<t when=\"1\" while=\"0\" />", true, true)]
    [InlineData("<t When=\"1\" WHILE=\"0\" />", true, true)]
    [InlineData("<t while=\"1\" when=\"0\" />", true, true)]
    [InlineData("<t a=\"1\" while=\"1\" b=\"1\" when=\"0\" c=\"\" />", true, true)]
    [InlineData("<t when=\"1\" />", true, false)]
    [InlineData("<T WHEN=\"1\" />", true, false)]
    [InlineData("<t while=\"0\" />", false, true)]
    [InlineData("<t WHile=\"0\" />", false, true)]
    [InlineData("<t />", false, false)]
    [InlineData("<t abc=\"1\" />", false, false)]
    public async Task AttributeParsing(string xml, bool hasWhen, bool hasWhile)
    {
        using var textReader = new StringReader(xml);
        using var reader = XmlReader.Create(textReader, new() { Async = true });
        reader.MoveToContent();

        var context = A.Dummy<IParsingContext>();
        var whenAttribute = A.Dummy<WhenAttribute>();
        var whileAttribute = A.Dummy<WhileAttribute>();

        var whenParser = A.Fake<IAttributeParser>();
        A.CallTo(() => whenParser.Parse(reader, context)).Returns(whenAttribute);

        var whileParser = A.Fake<IAttributeParser>();
        A.CallTo(() => whileParser.Parse(reader, context)).Returns(whileAttribute);

        var sut = new ElementParser();
        sut.AttributeParsers.Add("when", whenParser);
        sut.AttributeParsers.Add("while", whileParser);

        var parsed = await sut.ParseAsync(reader, context);

        parsed.Should().NotBeNull();
        parsed.Text.Should().BeNull();
        parsed.Block.Should().BeNull();
        
        if (hasWhen)
            parsed.When.Should().BeSameAs(whenAttribute.Condition);
        else
            parsed.When.Should().BeNull();
        
        if (hasWhile)
            parsed.While.Should().BeSameAs(whileAttribute.Condition);
        else
            parsed.While.Should().BeNull();
    }

    [Fact]
    public async Task UnconfiguredAttribute()
    {
        const string whenMessage = "Atributo não reconhecido: when";
        const string whileMessage = "Atributo não reconhecido: while";

        using var textReader = new StringReader("<teste when=\"1\" while=\"0\" />");
        using var reader = XmlReader.Create(textReader, new() { Async = true });
        reader.MoveToContent();

        var context = A.Fake<IParsingContext>(i => i.Strict());
        A.CallTo(() => context.LogError(reader, whenMessage)).DoesNothing();
        A.CallTo(() => context.LogError(reader, whileMessage)).DoesNothing();

        var sut = new ElementParser();

        var parsed = await sut.ParseAsync(reader, context);

        parsed.Should().NotBeNull();

        A.CallTo(() => context.LogError(reader, whenMessage)).MustHaveHappenedOnceExactly();
        A.CallTo(() => context.LogError(reader, whileMessage)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [InlineData("<t><a /><b /></t>", true, true)]
    [InlineData("<t><a x=\"1\" /><b /></t>", true, true)]
    [InlineData("<t><A /><B Y=\"0\" /></t>", true, true)]
    [InlineData("<t><x /><a /><y /><b /><z /></t>", true, true)]
    [InlineData("<t><a /></t>", true, false)]
    [InlineData("<t><b /></t>", false, true)]
    [InlineData("<t></t>", false, false)]
    public async Task ElementParsing(string xml, bool hasA, bool hasB)
    {
        using var textReader = new StringReader(xml);
        using var reader = XmlReader.Create(textReader, new() { Async = true });
        reader.MoveToContent();

        var context = A.Dummy<IParsingContext>();

        var aNode = A.Dummy<INode>();
        var bNode = A.Dummy<INode>();
        var aParser = new DummyNodeParser(aNode);

        var sut = new ElementParser();
        sut.ChildParsers.Add("a", new DummyNodeParser(aNode));
        sut.ChildParsers.Add("b", new DummyNodeParser(bNode));

        var parsed = await sut.ParseAsync(reader, context);

        parsed.Should().NotBeNull();
        parsed.Text.Should().BeNull();
        parsed.When.Should().BeNull();
        parsed.While.Should().BeNull();

        if (hasA || hasB)
        {
            parsed.Block.Should().NotBeNull();

            if (hasA)
                parsed.Block.ForwardQueue.Dequeue().Should().BeSameAs(aNode);
            if (hasB)
                parsed.Block.ForwardQueue.Dequeue().Should().BeSameAs(bNode);

            parsed.Block.ForwardQueue.Count.Should().Be(0);
        }
        else
        {
            parsed.Block.Should().BeNull();
        }        
    }

    private class DummyNodeParser : INodeParser
    {
        private readonly INode ret;

        public DummyNodeParser(INode ret)
        {
            this.ret = ret;
        }

        public string ElementName => throw new NotImplementedException();

        public async Task<INode> ParseAsync(XmlReader reader, IParsingContext parsingContext)
        {
            while (await reader.ReadAsync())
            {
            }
            return ret;
        }
    }
}
