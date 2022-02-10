﻿using IS.Reading.Nodes;
using IS.Reading.Variables;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.BalloonParsers;

public class BalloonChildNodeParserBaseTests
{
    private class TestClass : BalloonChildNodeParserBase
    {
        public TestClass(
            IElementParser elementParser, 
            ITextSourceParser textSourceParser, 
            IBalloonTextNodeParser balloonTextNodeParser, 
            IChoiceNodeParser choiceNodeParser
        ) 
            : base(elementParser, textSourceParser, balloonTextNodeParser, choiceNodeParser)
        {
        }
    }

    private readonly IElementParser elementParser;
    private readonly ITextSourceParser textSourceParser;
    private readonly IBalloonTextNodeParser balloonTextNodeParser;
    private readonly IChoiceNodeParser choiceNodeParser;
    private readonly BalloonType balloonType = BalloonType.Narration;
    private readonly ITextSource textSource;
    private readonly TestClass sut;

    public BalloonChildNodeParserBaseTests()
    {
        textSource = A.Dummy<ITextSource>();
        var textSourceParserResult = A.Fake<ITextSourceParserResult>(i => i.Strict());
        A.CallTo(() => textSourceParserResult.IsError).Returns(false);
        A.CallTo(() => textSourceParserResult.TextSource).Returns(textSource);

        textSourceParser = A.Fake<ITextSourceParser>(i => i.Strict());
        A.CallTo(() => textSourceParser.Parse(A<string>.Ignored)).Returns(textSourceParserResult);

        elementParser = A.Fake<IElementParser>(i => i.Strict());
        balloonTextNodeParser = Helper.FakeParser<IBalloonTextNodeParser>("xyz");
        A.CallTo(() => balloonTextNodeParser.BalloonType).Returns(balloonType);
        choiceNodeParser = Helper.FakeParser<IChoiceNodeParser>("choice");
        sut = new (elementParser, textSourceParser, balloonTextNodeParser, choiceNodeParser);
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("xyz");
        sut.BalloonType.Should().Be(BalloonType.Narration);
        sut.Settings.ShouldBeAggregatedNonRepeat(balloonTextNodeParser);
        sut.AggregationSettings.ShouldBeAggregatedNonRepeat(choiceNodeParser);
    }

    [Fact]
    public async Task ShouldReturnNodeWhenParsedTextIsNotNull()
    {
        var reader = A.Dummy<XmlReader>();
        var context = A.Dummy<IParsingContext>();
        var parentContext = new FakeParentParsingContext();

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.Arguments.Get<IParentParsingContext>(2).ParsedText = "abc");

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.AggregationSettings))
            .DoesNothing();

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.ShouldContainSingle<BalloonTextNode>(i => {
            i.TextSource.Should().BeSameAs(textSource);
            i.BalloonType.Should().Be(balloonType);
        });
    }

    [Fact]
    public async Task ShouldReturnNullWhenParsedTextIsNull()
    {
        var reader = A.Dummy<XmlReader>();
        var context = A.Dummy<IParsingContext>();
        var parentContext = new FakeParentParsingContext();

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings)).DoesNothing();

        await sut.ParseAsync(reader, context, parentContext);
        parentContext.ShouldBeEmpty();
    }

    [Fact]
    public async Task ShouldResetSceneParsingContext()
    {
        var reader = A.Dummy<XmlReader>();        
        var parentContext = new FakeParentParsingContext();

        var sceneContext = A.Fake<IParsingSceneContext>(i => i.Strict());
        A.CallTo(() => sceneContext.Reset()).DoesNothing();

        var context = A.Fake<IParsingContext>(i => i.Strict());
        A.CallTo(() => context.SceneContext).Returns(sceneContext);

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.Arguments.Get<IParentParsingContext>(2).ParsedText = "abc");

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.AggregationSettings))
            .DoesNothing();

        await sut.ParseAsync(reader, context, parentContext);

        A.CallTo(() => context.SceneContext).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ShouldLogTextSourceParsingError()
    {
        var errorMessage = "Erro proposital.";
        var reader = A.Dummy<XmlReader>();
        var parentContext = new FakeParentParsingContext();

        var parsingResult = A.Dummy<ITextSourceParserResult>();
        A.CallTo(() => parsingResult.IsError).Returns(true);
        A.CallTo(() => parsingResult.ErrorMessage).Returns(errorMessage);

        A.CallTo(() => textSourceParser.Parse("abc")).Returns(parsingResult);

        var context = A.Fake<IParsingContext>(i => i.Strict());
        A.CallTo(() => context.LogError(reader, errorMessage)).DoesNothing();

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.Arguments.Get<IParentParsingContext>(2).ParsedText = "abc");

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.AggregationSettings))
            .DoesNothing();

        await sut.ParseAsync(reader, context, parentContext);

        A.CallTo(() => context.LogError(reader, errorMessage)).MustHaveHappenedOnceExactly();
    }
}