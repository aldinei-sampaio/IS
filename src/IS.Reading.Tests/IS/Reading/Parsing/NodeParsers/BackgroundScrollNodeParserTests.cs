﻿using IS.Reading.Nodes;
using IS.Reading.Parsing.AttributeParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers;

public class BackgroundScrollNodeParserTests
{
    private readonly XmlReader reader;
    private readonly IParsingContext context;
    private readonly IElementParser elementParser;
    private readonly IWhenAttributeParser whenAttributeParser;
    private readonly BackgroundScrollNodeParser sut;

    public BackgroundScrollNodeParserTests()
    {
        reader = A.Dummy<XmlReader>();
        context = A.Fake<IParsingContext>(i => i.Strict());
        elementParser = A.Fake<IElementParser>(i => i.Strict());
        whenAttributeParser = Helper.FakeParser<IWhenAttributeParser>("when");

        sut = new(elementParser, whenAttributeParser);
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("scroll");
        sut.Settings.AttributeParsers["when"].Should().BeSameAs(whenAttributeParser);
        sut.Settings.AttributeParsers.Count.Should().Be(1);
        sut.Settings.ChildParsers.Count.Should().Be(0);
        sut.Settings.TextParser.Should().BeNull();
    }

    [Fact]
    public async Task Success()
    {
        var parsed = A.Dummy<IElementParsedData>();
        parsed.Text = null;
        A.CallTo(() => elementParser.ParseAsync(reader, context, sut.Settings)).Returns(parsed);

        var result = await sut.ParseAsync(reader, context);

        result.Should().NotBeNull();
        result.Should().BeOfType<BackgroundScrollNode>();

        var node = (BackgroundScrollNode)result;
        node.When.Should().BeSameAs(parsed.When);
    }
}