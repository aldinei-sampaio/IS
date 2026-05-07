using IS.Reading.Nodes;
using IS.Reading.Parsing.ArgumentParsers;
using IS.Reading.State;

namespace IS.Reading.Parsing.NodeParsers.BackgroundParsers;

public class BackgroundColorNodeParserTests
{
    private readonly IDocumentReader documentReader;
    private readonly IParsingContext parsingContext;
    private readonly FakeParentParsingContext parentParsingContext;

    private readonly IBackgroundColorArgumentParser backgroundColorArgumentParser;
    private readonly BackgroundColorNodeParser sut;

    public BackgroundColorNodeParserTests()
    {
        backgroundColorArgumentParser = A.Fake<IBackgroundColorArgumentParser>(i => i.Strict());
        sut = new(backgroundColorArgumentParser);

        documentReader = A.Fake<IDocumentReader>(i => i.Strict());
        parsingContext = Helper.FakeParsingContext();
        parentParsingContext = new FakeParentParsingContext();
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("color");
        sut.IsArgumentRequired.Should().BeTrue();
        sut.BackgroundColorArgumentParser.Should().BeSameAs(backgroundColorArgumentParser);
    }

    [Fact]
    public async Task ShouldLogArgumentParserError()
    {
        var errorMessage = "Erro proposital.";
        var argument = "salmon";
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => backgroundColorArgumentParser.Parse(argument)).Returns(Result.Fail<BackgroundColorArgument>(errorMessage));
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldBeEmpty();
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ParsingSuccess()
    {
        var argument = "black";

        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => backgroundColorArgumentParser.Parse(argument))
            .Returns(Result.Ok(new BackgroundColorArgument("black", BackgroundAnimation.None, null)));

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        var expectedState = new BackgroundState("black", BackgroundType.Color, BackgroundPosition.Undefined);

        parentParsingContext.ShouldContainSingle<BackgroundNode>(i =>
        {
            i.State.Should().Be(expectedState);
            i.Animation.Should().Be(BackgroundAnimation.None);
            i.FlashColor.Should().BeNull();
        });
    }

    [Theory]
    [InlineData(BackgroundAnimation.FadeIn,   null)]
    [InlineData(BackgroundAnimation.Zoom,     null)]
    [InlineData(BackgroundAnimation.Dissolve, null)]
    [InlineData(BackgroundAnimation.Flash,    null)]
    [InlineData(BackgroundAnimation.Flash,    "white")]
    public async Task ParsingWithAnimation(BackgroundAnimation animation, string? flashColor)
    {
        var argument = $"black {animation}";

        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => backgroundColorArgumentParser.Parse(argument))
            .Returns(Result.Ok(new BackgroundColorArgument("black", animation, flashColor)));

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldContainSingle<BackgroundNode>(i =>
        {
            i.Animation.Should().Be(animation);
            i.FlashColor.Should().Be(flashColor);
        });
    }
}
