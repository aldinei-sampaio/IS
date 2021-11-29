﻿using IS.Reading.Navigation;
using IS.Reading.Parsing.AttributeParsers;
using System.Xml;

namespace IS.Reading.Parsing;

public class ElementParserTests
{
    [Fact]
    public async Task Empty()
    {
        using var reader = Helper.CreateReader("<teste />");
        var context = A.Dummy<IParsingContext>();
        var settings = A.Dummy<IElementParserSettings>();

        var sut = new ElementParser();
        var parsed = await sut.ParseAsync(reader, context, settings);

        parsed.Should().NotBeNull();
        parsed.Text.Should().BeNull();
        parsed.Block.Should().BeNull();
        parsed.When.Should().BeNull();
        parsed.While.Should().BeNull();
    }

    private static IElementParserSettings FakeSettings(params object[] parsers)
    {
        var settings = A.Dummy<IElementParserSettings>();

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
                settings.TextParser = textParser;
            }
        }

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


        var whenParser = Helper.FakeParser<IAttributeParser>("when");
        A.CallTo(() => whenParser.Parse(reader, context)).Returns(whenAttribute);

        var whileParser = Helper.FakeParser<IAttributeParser>("while");
        A.CallTo(() => whileParser.Parse(reader, context)).Returns(whileAttribute);

        var settings = FakeSettings(whenParser, whileParser);

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

        using var reader = Helper.CreateReader("<teste when=\"1\" while=\"0\" />");

        var context = A.Fake<IParsingContext>(i => i.Strict());
        A.CallTo(() => context.LogError(reader, whenMessage)).DoesNothing();
        A.CallTo(() => context.LogError(reader, whileMessage)).DoesNothing();

        var settings = A.Dummy<IElementParserSettings>();

        var sut = new ElementParser();
        var parsed = await sut.ParseAsync(reader, context, settings);

        parsed.Should().NotBeNull();

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

        var context = A.Fake<IParsingContext>(i => i.Strict());
        A.CallTo(() => context.IsSuccess).Returns(true);

        var aNode = A.Dummy<INode>();
        var bNode = A.Dummy<INode>();

        var settings = FakeSettings(
            new DummyNodeParser("a", aNode), 
            new DummyNodeParser("b", bNode)
        );

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

        using var reader = Helper.CreateReader("<t><a /><b /><c /></t>");

        var context = A.Fake<IParsingContext>(i => i.Strict());
        A.CallTo(() => context.LogError(reader, aMessage)).DoesNothing();
        A.CallTo(() => context.LogError(reader, bMessage)).DoesNothing();

        var node = A.Dummy<INode>();
        var settings = FakeSettings(new DummyNodeParser("c", node));

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
        using var reader = Helper.CreateReader("<t>Pindamonhangaba</t>");

        var context = A.Fake<IParsingContext>(i => i.Strict());
        
        var textParser = A.Fake<ITextParser>(i => i.Strict());
        A.CallTo(() => textParser.Parse(reader, context, "Pindamonhangaba")).Returns("Formatado");

        var settings = A.Fake<IElementParserSettings>();
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
    [InlineData("<t><![CDATA[ seção CDATA ]]></t>", "Conteúdo inválido detectado: CDATA")]
    public async Task InvalidContent(string xmlContent, string message)
    {
        using var reader = Helper.CreateReader(xmlContent);
        var context = A.Fake<IParsingContext>(i => i.Strict());
        A.CallTo(() => context.LogError(reader, message)).DoesNothing();
        var textParser = A.Fake<ITextParser>(i => i.Strict());

        var settings = A.Fake<IElementParserSettings>();
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

        using var reader = Helper.CreateReader("<t>Pindamonhangaba</t>");

        var context = A.Fake<IParsingContext>(i => i.Strict());
        A.CallTo(() => context.LogError(reader, message)).DoesNothing();

        var settings = A.Dummy<IElementParserSettings>();
        settings.TextParser = null;

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

        using var reader = Helper.CreateReader(xml);

        var context = A.Fake<IParsingContext>(i => i.Strict());
        A.CallTo(() => context.LogError(reader, message)).DoesNothing();

        var textParser = A.Fake<ITextParser>(i => i.Strict());
        A.CallTo(() => textParser.Parse(reader, context, "abc")).Returns("def");

        var node = A.Dummy<INode>();
        var settings = FakeSettings(textParser, new DummyNodeParser("a", node));

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

    [Fact]
    public async Task DismissNode()
    {
        using var reader = Helper.CreateReader("<a><b /><b /></a>");
        var context = A.Fake<IParsingContext>(i => i.Strict());
        var dismissNodes = new List<INode>();
        A.CallTo(() => context.DismissNodes).Returns(dismissNodes);

        var normalNode = A.Dummy<INode>();
        var dismissNode = A.Dummy<INode>();

        var parser1 = Helper.FakeNodeParser("b");
        A.CallTo(() => parser1.ParseAsync(A<XmlReader>.Ignored, context)).Returns(normalNode);
        A.CallTo(() => parser1.DismissNode).Returns(dismissNode);

        var settings = FakeSettings(parser1);

        var sut = new ElementParser();
        var parsed = await sut.ParseAsync(reader, context, settings);

        parsed.Block.ForwardQueue.Count.Should().Be(2);
        parsed.Block.ForwardQueue.Dequeue().Should().BeSameAs(normalNode);
        parsed.Block.ForwardQueue.Dequeue().Should().BeSameAs(normalNode);

        dismissNodes.Count.Should().Be(1);
        dismissNodes[0].Should().BeSameAs(dismissNode);
    }

    [Fact]
    public async Task AggregatedNodes()
    {
        var aNode = A.Dummy<INode>();
        var bNode = A.Dummy<INode>();

        var bParser = new DummyNodeParser("b", bNode);
        var aParser = new DummyNodeParser("a", aNode, bParser);
        var settings = new DummySettings(aParser);

        using var reader = Helper.CreateReader("<sb><a /><b /></sb>");
        var context = A.Dummy<IParsingContext>();

        var sut = new ElementParser();
        var ret = await sut.ParseAsync(reader, context, settings);

        ret.Should().NotBeNull();
        ret.Block.ForwardQueue.Count.Should().Be(1);
        var finalNode = ret.Block.ForwardQueue.Dequeue();
        var queue = finalNode.ChildBlock.ForwardQueue;
        queue.Count().Should().Be(2);
        queue.Dequeue().Should().BeSameAs(aNode);
        queue.Dequeue().Should().BeSameAs(bNode);
    }

    [Fact]
    public async Task AggregatedAndNormal()
    {
        var aNode = A.Dummy<INode>();
        var bNode = A.Dummy<INode>();
        var cNode = A.Dummy<INode>();

        var bParser = new DummyNodeParser("b", bNode);
        var aParser = new DummyNodeParser("a", aNode, bParser);
        var cParser = new DummyNodeParser("c", cNode);
        var settings = new DummySettings(aParser, cParser);

        using var reader = Helper.CreateReader("<sb><a /><b /><c /></sb>");
        var context = A.Dummy<IParsingContext>();

        var sut = new ElementParser();
        var ret = await sut.ParseAsync(reader, context, settings);

        ret.Should().NotBeNull();
        ret.Block.ForwardQueue.Count.Should().Be(2);
        var finalNode = ret.Block.ForwardQueue.Dequeue();
        var queue = finalNode.ChildBlock.ForwardQueue;
        queue.Count().Should().Be(2);
        queue.Dequeue().Should().BeSameAs(aNode);
        queue.Dequeue().Should().BeSameAs(bNode);

        ret.Block.ForwardQueue.Dequeue().Should().BeSameAs(cNode);
    }

    [Fact]
    public async Task AggregateRepeated()
    {
        var aNode = A.Dummy<INode>();
        var a2Node = A.Dummy<INode>();
        var cNode = A.Dummy<INode>();

        var a2Parser = new DummyNodeParser("a", a2Node);
        var aParser = new DummyNodeParser("a", aNode, a2Parser);
        var cParser = new DummyNodeParser("c", cNode);
        var settings = new DummySettings(aParser, cParser);

        using var reader = Helper.CreateReader("<sb><a /><a /><a /><a /><c /></sb>");
        var context = A.Dummy<IParsingContext>();

        var sut = new ElementParser();
        var ret = await sut.ParseAsync(reader, context, settings);

        ret.Should().NotBeNull();
        ret.Block.ForwardQueue.Count.Should().Be(2);
        var finalNode = ret.Block.ForwardQueue.Dequeue();
        var queue = finalNode.ChildBlock.ForwardQueue;
        queue.Count().Should().Be(4);
        queue.Dequeue().Should().BeSameAs(aNode);
        queue.Dequeue().Should().BeSameAs(a2Node);
        queue.Dequeue().Should().BeSameAs(a2Node);
        queue.Dequeue().Should().BeSameAs(a2Node);

        ret.Block.ForwardQueue.Dequeue().Should().BeSameAs(cNode);
    }

    [Fact]
    public async Task AggregateNullParsed()
    {
        var a2Parser = new DummyNodeParser("a", null);
        var aParser = new DummyNodeParser("a", null, a2Parser);
        var settings = new DummySettings(aParser);

        using var reader = Helper.CreateReader("<sb><a /><a /></sb>");
        var context = A.Dummy<IParsingContext>();

        var sut = new ElementParser();
        var ret = await sut.ParseAsync(reader, context, settings);

        ret.Should().NotBeNull();
        ret.Block.Should().BeNull();
    }

    [Fact]
    public async Task AggregatedReturningNull()
    {
        var aNode = A.Dummy<INode>();
        var bNode = A.Dummy<INode>();

        var bParser = new DummyNodeParser("b", bNode);
        var aParser = new DummyNodeParser("a", aNode, bParser);
        aParser.AggregateShouldReturnNull = true;
        var settings = new DummySettings(aParser);

        using var reader = Helper.CreateReader("<sb><a /><b /></sb>");
        var context = A.Dummy<IParsingContext>();

        var sut = new ElementParser();
        var ret = await sut.ParseAsync(reader, context, settings);

        ret.Should().NotBeNull();
        ret.Block.Should().BeNull();
    }

    private class DummySettings : IElementParserSettings
    {
        public DummySettings(params INodeParser[] nodeParsers)
        {
            foreach(var nodeParser in nodeParsers)
                ChildParsers.Add(nodeParser);
        }

        public ITextParser TextParser { get; set; }

        public IParserDictionary<IAttributeParser> AttributeParsers { get; }
            = new ParserDictionary<IAttributeParser>();

        public IParserDictionary<INodeParser> ChildParsers { get; }
            = new ParserDictionary<INodeParser>();
    }

    private class DummyAggregation : INodeAggregation
    {
        public IParserDictionary<INodeParser> ChildParsers { get; } 
            = new ParserDictionary<INodeParser>();
    }

    private class DummyNodeParser : INodeParser
    {
        private readonly INode ret;

        public DummyNodeParser(string name, INode ret, params INodeParser[] aggregated)
        {
            this.ret = ret;
            Name = name;
            if (aggregated is not null && aggregated.Length > 0)
            {
                var aggregation = new DummyAggregation();
                foreach (var aggParser in aggregated)
                    aggregation.ChildParsers.Add(aggParser);
                NodeAggregation = aggregation;
            }
            else
            {
                NodeAggregation = null;
            }
        }

        public INodeAggregation NodeAggregation { get; }

        public string Name { get; }

        public async Task<INode> ParseAsync(XmlReader reader, IParsingContext parsingContext)
        {
            while (await reader.ReadAsync())
            {
            }
            return ret;
        }

        public bool AggregateShouldReturnNull { get; set; }

        public INode Aggregate(IBlock block)
        {
            if (NodeAggregation is null)
                throw new InvalidOperationException("Aggregate() sendo chamado quando NodeAggregation é null");

            if (AggregateShouldReturnNull)
                return null;

            var node = A.Dummy<INode>();
            A.CallTo(() => node.ChildBlock).Returns(block);
            return node;
        }
    }
}
