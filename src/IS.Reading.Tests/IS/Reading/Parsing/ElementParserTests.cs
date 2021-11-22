using IS.Reading.Navigation;
using IS.Reading.Parsing.AttributeParsers;
using System.Xml;

namespace IS.Reading.Parsing;

public class ElementParserTests
{
    [Fact]
    public async Task Empty()
    {
        using var reader = CreateReader("<teste />");
        var context = A.Dummy<IParsingContext>();
        var settings = new DummySettings();

        var sut = new ElementParser();
        var parsed = await sut.ParseAsync(reader, context, settings);

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
        using var reader = CreateReader(xml);

        var context = A.Dummy<IParsingContext>();
        var whenAttribute = A.Dummy<WhenAttribute>();
        var whileAttribute = A.Dummy<WhileAttribute>();

        var whenParser = A.Fake<IAttributeParser>();
        A.CallTo(() => whenParser.Parse(reader, context)).Returns(whenAttribute);

        var whileParser = A.Fake<IAttributeParser>();
        A.CallTo(() => whileParser.Parse(reader, context)).Returns(whileAttribute);

        var settings = new DummySettings();
        settings.AttributeParsers.Add("when", whenParser);
        settings.AttributeParsers.Add("while", whileParser);

        var sut = new ElementParser();
        var parsed = await sut.ParseAsync(reader, context, settings);

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

        using var reader = CreateReader("<teste when=\"1\" while=\"0\" />");

        var context = A.Fake<IParsingContext>(i => i.Strict());
        A.CallTo(() => context.LogError(reader, whenMessage)).DoesNothing();
        A.CallTo(() => context.LogError(reader, whileMessage)).DoesNothing();

        var settings = new DummySettings();

        var sut = new ElementParser();
        var parsed = await sut.ParseAsync(reader, context, settings);

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
        using var reader = CreateReader(xml);

        var context = A.Dummy<IParsingContext>();

        var aNode = A.Dummy<INode>();
        var bNode = A.Dummy<INode>();
        var aParser = new DummyNodeParser(aNode);

        var settings = new DummySettings();
        settings.ChildParsers.Add("a", new DummyNodeParser(aNode));
        settings.ChildParsers.Add("b", new DummyNodeParser(bNode));

        var sut = new ElementParser();
        var parsed = await sut.ParseAsync(reader, context, settings);

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

    [Fact]
    public async Task UnconfiguredElement()
    {
        const string aMessage = "Elemento não reconhecido: a";
        const string bMessage = "Elemento não reconhecido: b";

        using var reader = CreateReader("<t><a /><b /><c /></t>");

        var context = A.Fake<IParsingContext>(i => i.Strict());
        A.CallTo(() => context.LogError(reader, aMessage)).DoesNothing();
        A.CallTo(() => context.LogError(reader, bMessage)).DoesNothing();

        var node = A.Dummy<INode>();
        var settings = new DummySettings();
        settings.ChildParsers.Add("c", new DummyNodeParser(node));

        var sut = new ElementParser();
        var parsed = await sut.ParseAsync(reader, context, settings);

        parsed.Should().NotBeNull();
        parsed.Block.Should().NotBeNull();
        parsed.Block.ForwardQueue.Peek().Should().BeSameAs(node);

        A.CallTo(() => context.LogError(reader, aMessage)).MustHaveHappenedOnceExactly();
        A.CallTo(() => context.LogError(reader, bMessage)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task TextParsing()
    {
        using var reader = CreateReader("<t>Pindamonhangaba</t>");

        var context = A.Fake<IParsingContext>(i => i.Strict());
        
        var textParser = A.Fake<ITextParser>(i => i.Strict());
        A.CallTo(() => textParser.Parse(reader, context, "Pindamonhangaba")).Returns("Formatado");

        var settings = new DummySettings();
        settings.TextParser = textParser;

        var sut = new ElementParser();
        var parsed = await sut.ParseAsync(reader, context, settings);

        parsed.Should().NotBeNull();
        parsed.When.Should().BeNull();
        parsed.While.Should().BeNull();
        parsed.Block.Should().BeNull();
        parsed.Text.Should().Be("Formatado");
    }

    [Theory]
    [InlineData("<t><!-- comentário --></t>", "Conteúdo inválido detectado: Comment")]
    [InlineData("<t><![CDATA[ seção CDATA ]]></t>", "Conteúdo inválido detectado: CDATA")]
    public async Task InvalidContent(string xmlContent, string message)
    {
        using var reader = CreateReader(xmlContent);
        var context = A.Fake<IParsingContext>(i => i.Strict());
        A.CallTo(() => context.LogError(reader, message)).DoesNothing();
        var textParser = A.Fake<ITextParser>(i => i.Strict());

        var settings = new DummySettings();
        settings.TextParser = textParser;

        var sut = new ElementParser();
        var parsed = await sut.ParseAsync(reader, context, settings);

        parsed.Should().NotBeNull();
        parsed.When.Should().BeNull();
        parsed.While.Should().BeNull();
        parsed.Block.Should().BeNull();
        parsed.Text.Should().BeNull();

        A.CallTo(() => context.LogError(reader, message)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task UnconfiguredText()
    {
        const string message = "Este elemento não permite texto.";

        using var reader = CreateReader("<t>Pindamonhangaba</t>");

        var context = A.Fake<IParsingContext>(i => i.Strict());
        A.CallTo(() => context.LogError(reader, message)).DoesNothing();

        var settings = new DummySettings();

        var sut = new ElementParser();
        var parsed = await sut.ParseAsync(reader, context, settings);

        parsed.Should().NotBeNull();
        parsed.When.Should().BeNull();
        parsed.While.Should().BeNull();
        parsed.Block.Should().BeNull();
        parsed.Text.Should().BeNull();

        A.CallTo(() => context.LogError(reader, message)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [InlineData("<t>abc<a /></t>", true)]
    [InlineData("<t><a />abc</t>", false)]
    public async Task TextAndElementSimultaneouslyIsNotAllowed(string xml, bool textFirst)
    {
        const string message = "Não é permitido texto dentro de elemento que tenha elementos filhos.";

        using var reader = CreateReader(xml);

        var context = A.Fake<IParsingContext>(i => i.Strict());
        A.CallTo(() => context.LogError(reader, message)).DoesNothing();

        var textParser = A.Fake<ITextParser>(i => i.Strict());
        A.CallTo(() => textParser.Parse(reader, context, "abc")).Returns("def");

        var node = A.Dummy<INode>();
        var settings = new DummySettings();
        settings.TextParser = textParser;
        settings.ChildParsers.Add("a", new DummyNodeParser(node));

        var sut = new ElementParser();
        var parsed = await sut.ParseAsync(reader, context, settings);

        parsed.Should().NotBeNull();
        parsed.When.Should().BeNull();
        parsed.While.Should().BeNull();
        if (textFirst)
        {
            parsed.Block.Should().BeNull();
            parsed.Text.Should().Be("def");
        }
        else
        {
            parsed.Block.Should().NotBeNull();
            parsed.Block.ForwardQueue.Peek().Should().BeEquivalentTo(node);
            parsed.Text.Should().BeNull();
        }

        A.CallTo(() => context.LogError(reader, message)).MustHaveHappenedOnceExactly();
    }

    private class DummySettings : IElementParserSettings
    {
        public ITextParser TextParser { get; set; }

        public ParserDictionary<IAttributeParser> AttributeParsers { get; } = new();

        public ParserDictionary<INodeParser> ChildParsers { get; } = new();
    }

    private static XmlReader CreateReader(string xmlContents)
    {
        var textReader = new StringReader(xmlContents);
        var reader = XmlReader.Create(textReader, new() { Async = true });
        reader.MoveToContent();
        return reader;
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
