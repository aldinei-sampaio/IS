﻿using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.AttributeParsers;
using IS.Reading.Parsing.TextParsers;
using IS.Reading.State;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.BackgroundParsers;

public class BackgroundColorNodeParserTests
{
    private readonly XmlReader reader;
    private readonly IParsingContext context;
    private readonly FakeParentParsingContext parentContext = new();
    private readonly IElementParser elementParser;
    private readonly IWhenAttributeParser whenAttributeParser;
    private readonly IColorTextParser colorTextParser;
    private readonly BackgroundColorNodeParser sut;

    public BackgroundColorNodeParserTests()
    {
        reader = A.Dummy<XmlReader>();
        context = A.Fake<IParsingContext>(i => i.Strict());
        elementParser = A.Fake<IElementParser>(i => i.Strict());
        whenAttributeParser = Helper.FakeParser<IWhenAttributeParser>("when");
        colorTextParser = A.Dummy<IColorTextParser>();

        sut = new(elementParser, whenAttributeParser, colorTextParser);
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("color");
        sut.Settings.AttributeParsers.Count.Should().Be(1);
        sut.Settings.AttributeParsers["when"].Should().BeSameAs(whenAttributeParser);
        sut.Settings.ChildParsers.Count.Should().Be(0);
        sut.Settings.TextParser.Should().BeSameAs(colorTextParser);
    }

    [Fact]
    public async Task Success()
    {
        var when = A.Dummy<ICondition>();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i =>
            {
                var ctx = i.Arguments.Get<IParentParsingContext>(2);
                ctx.ParsedText = "alfa";
                ctx.When = when;
            });

        await sut.ParseAsync(reader, context, parentContext);

        var node = parentContext.Nodes.Should().ContainSingle()
            .Which.Should().BeOfType<BackgroundNode>()
            .Which;

        node.When.Should().BeSameAs(when);
        node.State.Name.Should().Be("alfa");
        node.State.Type.Should().Be(BackgroundType.Color);
        node.State.Position.Should().Be(BackgroundPosition.Undefined);
    }

    [Fact]
    public async Task EmptyColorName()
    {
        const string message = "Era esperado o nome da cor.";
        A.CallTo(() => context.LogError(reader, message)).DoesNothing();

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.Arguments.Get<IParentParsingContext>(2).ParsedText = string.Empty);

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.ShouldBeEmpty();

        A.CallTo(() => context.LogError(reader, message)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task InvalidColorName()
    {
        // O ColorTextParser irá retornar NULL se o nome da cor for inválido
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.Arguments.Get<IParentParsingContext>(2).ParsedText = null);

        await sut.ParseAsync(reader, context, parentContext);
        parentContext.ShouldBeEmpty();
    }
}