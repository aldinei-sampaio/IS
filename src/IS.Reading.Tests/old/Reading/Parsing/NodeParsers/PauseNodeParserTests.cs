﻿using IS.Reading.Conditions;
using IS.Reading.Nodes;
using IS.Reading.Parsing.AttributeParsers;
using IS.Reading.Parsing.ArgumentParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers;

public class PauseNodeParserTests
{
    private readonly XmlReader reader;
    private readonly IParsingContext context;
    private readonly IParsingSceneContext sceneContext;
    private readonly FakeParentParsingContext parentContext = new();
    private readonly IElementParser elementParser;
    private readonly IWhenAttributeParser whenAttributeParser;
    private readonly IIntegerArgumentParser integerTextParser;
    private readonly PauseNodeParser sut;

    public PauseNodeParserTests()
    {
        reader = A.Dummy<XmlReader>();
        context = A.Fake<IParsingContext>(i => i.Strict());
        sceneContext = A.Fake<IParsingSceneContext>(i => i.Strict());
        A.CallTo(() => context.SceneContext).Returns(sceneContext);
        A.CallTo(() => sceneContext.Reset()).DoesNothing();

        elementParser = A.Fake<IElementParser>(i => i.Strict());
        whenAttributeParser = Helper.FakeParser<IWhenAttributeParser>("when");
        integerTextParser = A.Fake<IIntegerArgumentParser>();

        sut = new(elementParser, whenAttributeParser, integerTextParser);
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("pause");
        sut.Settings.AttributeParsers["when"].Should().BeSameAs(whenAttributeParser);
        sut.Settings.AttributeParsers.Count.Should().Be(1);
        sut.Settings.ChildParsers.Count.Should().Be(0);
        sut.Settings.TextParser.Should().BeSameAs(integerTextParser);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public async Task NormalPause(string text)
    {
        var when = A.Dummy<ICondition>();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => 
            {
                var ctx = i.Arguments.Get<IParentParsingContext>(2);
                ctx.ParsedText = text;
                ctx.When = when;
            });

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.ShouldContainSingle<PauseNode>(i => i.When.Should().BeSameAs(when));
        A.CallTo(() => sceneContext.Reset()).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(19)]
    [InlineData(258)]
    [InlineData(1234)]
    [InlineData(5000)]
    public async Task TimedPause(int value)
    {
        var when = A.Dummy<ICondition>();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i =>
            {
                var ctx = i.Arguments.Get<IParentParsingContext>(2);
                ctx.ParsedText = value.ToString();
                ctx.When = when;
            });

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.ShouldContainSingle<TimedPauseNode>(i =>
        {
            i.When.Should().BeSameAs(when);
            i.Duration.Should().Be(TimeSpan.FromMilliseconds(value));
        });
        A.CallTo(() => sceneContext.Reset()).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData("-5000")]
    public async Task DurationShouldBeGreaterThanZero(string duration)
    {
        const string message = "O tempo de espera precisa ser maior que zero.";
        await CheckForValidationErrorAsync(duration, message);
    }

    [Theory]
    [InlineData("5001")]
    [InlineData("6000")]
    [InlineData("10000")]
    [InlineData("999999999")]
    public async Task DurationShouldBeUpTo5000(string duration)
    {
        const string message = "O tempo de espera não pode ser maior que 5000.";
        await CheckForValidationErrorAsync(duration, message);
    }

    private async Task CheckForValidationErrorAsync(string duration, string message)
    {
        A.CallTo(() => context.LogError(reader, message)).DoesNothing();

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.Arguments.Get<IParentParsingContext>(2).ParsedText = duration);

        await sut.ParseAsync(reader, context, parentContext);
        parentContext.ShouldBeEmpty();

        A.CallTo(() => context.LogError(reader, message)).MustHaveHappenedOnceExactly();
    }
}