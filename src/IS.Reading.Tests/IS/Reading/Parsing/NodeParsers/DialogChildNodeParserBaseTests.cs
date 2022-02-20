using IS.Reading.Choices;
using IS.Reading.Nodes;
using IS.Reading.Variables;

namespace IS.Reading.Parsing.NodeParsers;

public class DialogChildNodeParserBaseTests
{
    private class TestClass : DialogChildNodeParserBase
    {
        public TestClass(
            IElementParser elementParser, 
            ITextSourceParser textSourceParser, 
            IChoiceNodeParser choiceNodeParser
        )
            : base(elementParser, textSourceParser, choiceNodeParser)
        {
        }

        public override string Name => "omega";
    }

    private readonly IElementParser elementParser;
    private readonly ITextSourceParser textSourceParser;
    private readonly IChoiceNodeParser choiceNodeParser;
    private readonly TestClass sut;

    private readonly IDocumentReader documentReader;
    private readonly IParsingContext parsingContext;
    private readonly BalloonParsingContext parentParsingContext;
    private readonly IParsingSceneContext parsingSceneContext;

    public DialogChildNodeParserBaseTests()
    {
        elementParser = A.Fake<IElementParser>(i => i.Strict());
        textSourceParser = A.Fake<ITextSourceParser>(i => i.Strict());
        choiceNodeParser = Helper.FakeParser<IChoiceNodeParser>("choice");
        sut = new(elementParser, textSourceParser, choiceNodeParser);

        documentReader = A.Fake<IDocumentReader>(i => i.Strict());
        parsingContext = A.Fake<IParsingContext>(i => i.Strict());
        parentParsingContext = new(BalloonType.Narration);
        parsingSceneContext = A.Fake<IParsingSceneContext>(i => i.Strict());
        A.CallTo(() => parsingContext.SceneContext).Returns(parsingSceneContext);
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("omega");
        sut.IsArgumentRequired.Should().BeTrue();
        sut.ElementParser.Should().BeSameAs(elementParser);
        sut.TextSourceParser.Should().BeSameAs(textSourceParser);
        sut.Settings.Should().BeOfType<ElementParserSettings.AggregatedNonRepeat>()
            .Which.ChildParsers.Should().BeEquivalentTo(choiceNodeParser);
    }

    [Fact]
    public async Task ShouldExitOnTextSourceParserError()
    {
        var errorMessage = "Erro proposital.";
        var argument = "Gibberish";
        A.CallTo(() => textSourceParser.Parse(argument)).Returns(Result.Fail<ITextSource>(errorMessage));
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ShouldExitOnElementParserError()
    {
        var argument = "Gibberish";
        var textSource = A.Dummy<ITextSource>();
        A.CallTo(() => textSourceParser.Parse(argument)).Returns(Result.Ok(textSource));
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => parsingContext.IsSuccess).Returns(false);
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.Settings))
            .DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.Nodes.Should().BeEmpty();
    }

    [Fact]
    public async Task ParsingSuccessWithoutChoice()
    {
        var argument = "Gibberish";
        var textSource = A.Dummy<ITextSource>();
        A.CallTo(() => textSourceParser.Parse(argument)).Returns(Result.Ok(textSource));
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.Settings))
            .DoesNothing();
        A.CallTo(() => parsingSceneContext.Reset()).DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.Nodes.Count.Should().Be(1);
        parentParsingContext.Nodes[0].Should().BeOfType<BalloonTextNode>()
            .Which.ShouldSatisfy(i =>
            {
                i.BalloonType.Should().Be(BalloonType.Narration);
                i.ChoiceBuilder.Should().BeNull();
                i.TextSource.Should().BeSameAs(textSource);
            });

        A.CallTo(() => parsingSceneContext.Reset()).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ParsingSuccessWithChoice()
    {
        var argument = "Gibberish";
        var textSource = A.Dummy<ITextSource>();
        var choiceBuilder = A.Dummy<IChoiceBuilder>();
        A.CallTo(() => textSourceParser.Parse(argument)).Returns(Result.Ok(textSource));
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.GetArgument<BalloonChildParsingContext>(2).ChoiceBuilder = choiceBuilder);
        A.CallTo(() => parsingSceneContext.Reset()).DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.Nodes.Count.Should().Be(1);
        parentParsingContext.Nodes[0].Should().BeOfType<BalloonTextNode>()
            .Which.ShouldSatisfy(i =>
            {
                i.BalloonType.Should().Be(BalloonType.Narration);
                i.ChoiceBuilder.Should().BeSameAs(choiceBuilder);
                i.TextSource.Should().BeSameAs(textSource);
            });

        A.CallTo(() => parsingSceneContext.Reset()).MustHaveHappenedOnceExactly();
    }
}