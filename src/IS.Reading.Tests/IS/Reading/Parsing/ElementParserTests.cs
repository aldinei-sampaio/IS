using IS.Reading.Navigation;
using IS.Reading.Parsing.AttributeParsers;

namespace IS.Reading.Parsing;

public class ElementParserTests
{
    [Fact]
    public async Task Empty()
    {
        using var reader = Helper.CreateReader("<teste />");
        var context = A.Fake<IParsingContext>(i => i.Strict());
        var settings = A.Fake<IElementParserSettings>(i => i.Strict());
        var parentContext = A.Fake<IParentParsingContext>(i => i.Strict());

        var sut = new ElementParser();
        await sut.ParseAsync(reader, context, parentContext, settings);
    }

    private static IElementParserSettings FakeSettings(params object[] parsers)
    {
        var settings = A.Dummy<IElementParserSettings>();

        var foundTextParser = false;
        foreach(var parser in parsers)
        {
            if (parser is INodeParser nodeParser)
            {
                INodeParser ignoredParser = null;
                A.CallTo(() => settings.ChildParsers.TryGet(nodeParser.Name, out ignoredParser))
                    .Returns(true)
                    .AssignsOutAndRefParameters(nodeParser);
            }
            else if (parser is IAttributeParser attributeParser)
            {
                IAttributeParser ignoredParser = null;
                A.CallTo(() => settings.AttributeParsers.TryGet(attributeParser.Name, out ignoredParser))
                    .Returns(true)
                    .AssignsOutAndRefParameters(attributeParser);
            }
            else if (parser is ITextParser textParser)
            {
                A.CallTo(() => settings.TextParser).Returns(textParser);
                foundTextParser = true;
            }
        }

        if (!foundTextParser)
            A.CallTo(() => settings.TextParser).Returns(null);

        return settings;
    }

    [Theory]
    [InlineData("<t when=\"1\" while=\"0\" />", true, true)]
    [InlineData("<t while=\"1\" when=\"0\" />", true, true)]
    [InlineData("<t a=\"1\" while=\"1\" b=\"1\" when=\"0\" c=\"\" />", true, true)]
    [InlineData("<t when=\"1\" />", true, false)]
    [InlineData("<t while=\"0\" />", false, true)]
    [InlineData("<t />", false, false)]
    [InlineData("<t abc=\"1\" />", false, false)]
    public async Task AttributeParsing(string xml, bool hasWhen, bool hasWhile)
    {
        using var reader = Helper.CreateReader(xml);

        var context = A.Dummy<IParsingContext>();
        var whenAttribute = A.Dummy<WhenAttribute>();
        var whileAttribute = A.Dummy<WhileAttribute>();
        var parentContext = new FakeParentParsingContext();

        var whenParser = Helper.FakeParser<IAttributeParser>("when");
        A.CallTo(() => whenParser.Parse(reader, context)).Returns(whenAttribute);

        var whileParser = Helper.FakeParser<IAttributeParser>("while");
        A.CallTo(() => whileParser.Parse(reader, context)).Returns(whileAttribute);

        var settings = FakeSettings(whenParser, whileParser);

        var sut = new ElementParser();
        await sut.ParseAsync(reader, context, parentContext, settings);

        parentContext.ParsedText.Should().BeNull();
        
        if (hasWhen)
            parentContext.When.Should().BeSameAs(whenAttribute.Condition);
        else
            parentContext.When.Should().BeNull();
        
        if (hasWhile)
            parentContext.While.Should().BeSameAs(whileAttribute.Condition);
        else
            parentContext.While.Should().BeNull();
    }

    [Fact]
    public async Task UnconfiguredAttribute()
    {
        const string whenMessage = "Atributo não reconhecido: when";
        const string whileMessage = "Atributo não reconhecido: while";

        using var reader = Helper.CreateReader("<teste when=\"1\" while=\"0\" />");

        var parentContext = new FakeParentParsingContext();
        var context = A.Fake<IParsingContext>(i => i.Strict());
        A.CallTo(() => context.LogError(reader, whenMessage)).DoesNothing();
        A.CallTo(() => context.LogError(reader, whileMessage)).DoesNothing();

        var settings = A.Dummy<IElementParserSettings>();

        var sut = new ElementParser();
        await sut.ParseAsync(reader, context, parentContext, settings);

        parentContext.When.Should().BeNull();
        parentContext.While.Should().BeNull();

        A.CallTo(() => context.LogError(reader, whenMessage)).MustHaveHappenedOnceExactly();
        A.CallTo(() => context.LogError(reader, whileMessage)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [InlineData("<t><a /><b /></t>", true, true)]
    [InlineData("<t>\r\n  <a />\r\n  <b />\r\n</t>", true, true)]
    [InlineData("<t><a x=\"1\" /><b /></t>", true, true)]
    [InlineData("<t><a /><b y=\"0\" /></t>", true, true)]
    [InlineData("<t><a /></t>", true, false)]
    [InlineData("<t><b /></t>", false, true)]
    [InlineData("<t><!-- Comentário --></t>", false, false)]
    public async Task ElementParsing(string xml, bool hasA, bool hasB)
    {
        using var reader = Helper.CreateReader(xml);

        var parentContext = new FakeParentParsingContext();
        var context = A.Fake<IParsingContext>(i => i.Strict());
        A.CallTo(() => context.IsSuccess).Returns(true);

        var aNode = A.Dummy<INode>();
        var bNode = A.Dummy<INode>();

        var settings = FakeSettings(
            new FakeNodeParser("a", aNode), 
            new FakeNodeParser("b", bNode)
        );

        var sut = new ElementParser();
        await sut.ParseAsync(reader, context, parentContext, settings);

        parentContext.ParsedText.Should().BeNull();
        parentContext.When.Should().BeNull();
        parentContext.While.Should().BeNull();

        if (hasA)
        {
            parentContext.Nodes.Should().Contain(aNode);
            parentContext.Nodes.Remove(aNode);
        }

        if (hasB)
        {
            parentContext.Nodes.Should().Contain(bNode);
            parentContext.Nodes.Remove(bNode);
        }

        parentContext.Nodes.Should().BeEmpty();
    }

    [Fact]
    public async Task UnconfiguredElement()
    {
        const string aMessage = "Elemento não reconhecido: a";
        const string bMessage = "Elemento não reconhecido: b";

        using var reader = Helper.CreateReader("<t><a /><b /><c /></t>");

        var parentContext = new FakeParentParsingContext();
        var context = A.Fake<IParsingContext>(i => i.Strict());
        A.CallTo(() => context.LogError(reader, aMessage)).DoesNothing();
        A.CallTo(() => context.LogError(reader, bMessage)).DoesNothing();

        var node = A.Dummy<INode>();
        var settings = FakeSettings(new FakeNodeParser("c", node));

        var sut = new ElementParser();
        await sut.ParseAsync(reader, context, parentContext, settings);

        parentContext.Nodes.Single().Should().BeSameAs(node);

        A.CallTo(() => context.LogError(reader, aMessage)).MustHaveHappenedOnceExactly();
        A.CallTo(() => context.LogError(reader, bMessage)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task TextParsing()
    {
        using var reader = Helper.CreateReader("<t>Pindamonhangaba</t>");

        var parentContext = new FakeParentParsingContext();
        var context = A.Fake<IParsingContext>(i => i.Strict());
        
        var textParser = A.Fake<ITextParser>(i => i.Strict());
        A.CallTo(() => textParser.Parse(reader, context, "Pindamonhangaba")).Returns("Formatado");

        var settings = FakeSettings(textParser);

        var sut = new ElementParser();
        await sut.ParseAsync(reader, context, parentContext, settings);

        parentContext.When.Should().BeNull();
        parentContext.While.Should().BeNull();
        parentContext.Nodes.Should().BeEmpty();
        parentContext.ParsedText.Should().Be("Formatado");
    }

    [Theory]
    [InlineData("<t><![CDATA[ seção CDATA ]]></t>", "Conteúdo inválido detectado: CDATA")]
    public async Task InvalidContent(string xmlContent, string message)
    {
        using var reader = Helper.CreateReader(xmlContent);
        var parentContext = new FakeParentParsingContext();
        var context = A.Fake<IParsingContext>(i => i.Strict());
        A.CallTo(() => context.LogError(reader, message)).DoesNothing();
        var textParser = A.Fake<ITextParser>(i => i.Strict());

        var settings = FakeSettings(textParser);

        var sut = new ElementParser();
        await sut.ParseAsync(reader, context, parentContext, settings);

        parentContext.ShouldBeEmpty();

        A.CallTo(() => context.LogError(reader, message)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task UnconfiguredText()
    {
        const string message = "Este elemento não permite texto.";

        using var reader = Helper.CreateReader("<t>Pindamonhangaba</t>");

        var parentContext = new FakeParentParsingContext(); 
        var context = A.Fake<IParsingContext>(i => i.Strict());
        A.CallTo(() => context.LogError(reader, message)).DoesNothing();

        var settings = FakeSettings();
        var sut = new ElementParser();
        await sut.ParseAsync(reader, context, parentContext, settings);

        parentContext.ShouldBeEmpty();

        A.CallTo(() => context.LogError(reader, message)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [InlineData("<t>abc<a /></t>", true)]
    [InlineData("<t><a />abc</t>", false)]
    public async Task TextAndElementSimultaneouslyIsNotAllowed(string xml, bool textFirst)
    {
        const string message = "Não é permitido texto dentro de elemento que tenha elementos filhos.";

        using var reader = Helper.CreateReader(xml);

        var parentContext = new FakeParentParsingContext();
        var context = A.Fake<IParsingContext>(i => i.Strict());
        A.CallTo(() => context.LogError(reader, message)).DoesNothing();

        var textParser = A.Fake<ITextParser>(i => i.Strict());
        A.CallTo(() => textParser.Parse(reader, context, "abc")).Returns("def");

        var node = A.Dummy<INode>();
        var settings = FakeSettings(textParser, new FakeNodeParser("a", node));

        var sut = new ElementParser();
        await sut.ParseAsync(reader, context, parentContext, settings);

        parentContext.When.Should().BeNull();
        parentContext.While.Should().BeNull();
        if (textFirst)
        {
            parentContext.Nodes.Should().BeEmpty();
            parentContext.ParsedText.Should().Be("def");
        }
        else
        {
            parentContext.Nodes.Should().ContainSingle().Which.Should().BeSameAs(node);
            parentContext.ParsedText.Should().BeNull();
        }

        A.CallTo(() => context.LogError(reader, message)).MustHaveHappenedOnceExactly();
    }

}
