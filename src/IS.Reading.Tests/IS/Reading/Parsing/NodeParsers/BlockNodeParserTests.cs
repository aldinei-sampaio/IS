using IS.Reading.Conditions;
using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.ConditionParsers;

namespace IS.Reading.Parsing.NodeParsers;

public class BlockNodeParserTests
{
    private readonly IDocumentReader documentReader;
    private readonly IParsingContext parsingContext;
    private readonly FakeParentParsingContext parentParsingContext = new();
    private readonly IElementParser elementParser;
    private readonly IMusicNodeParser musicNodeParser;
    private readonly IBackgroundNodeParser backgroundNodeParser;
    private readonly IPauseNodeParser pauseNodeParser;
    private readonly IMainCharacterNodeParser mainCharacterNodeParser;
    private readonly IPersonNodeParser personNodeParser;
    private readonly INarrationNodeParser narrationNodeParser;
    private readonly ITutorialNodeParser tutorialNodeParser;
    private readonly ISetNodeParser setNodeParser;
    private readonly IConditionParser conditionParser;
    private readonly BlockNodeParser sut;

    public BlockNodeParserTests()
    {
        documentReader = A.Dummy<IDocumentReader>();

        parsingContext = Helper.FakeParsingContext();
        elementParser = A.Fake<IElementParser>(i => i.Strict(StrictFakeOptions.AllowToString));
        musicNodeParser = Helper.FakeParser<IMusicNodeParser>("music");
        backgroundNodeParser = Helper.FakeParser<IBackgroundNodeParser>("background");
        pauseNodeParser = Helper.FakeParser<IPauseNodeParser>("pause");
        mainCharacterNodeParser = Helper.FakeParser<IMainCharacterNodeParser>("mc");
        personNodeParser = Helper.FakeParser<IPersonNodeParser>("person");
        narrationNodeParser = Helper.FakeParser<INarrationNodeParser>("narration");
        tutorialNodeParser = Helper.FakeParser<ITutorialNodeParser>("tutorial");
        setNodeParser = Helper.FakeParser<ISetNodeParser>("set");
        conditionParser = A.Fake<IConditionParser>(i => i.Strict(StrictFakeOptions.AllowToString));

        sut = new(
            elementParser, 
            conditionParser,
            musicNodeParser,
            backgroundNodeParser, 
            pauseNodeParser,
            mainCharacterNodeParser,
            personNodeParser,
            narrationNodeParser,
            tutorialNodeParser,
            setNodeParser
        );
    }

    [Fact]
    public void Initialization()
    {
        var parsers = new INodeParser[]
        {
            musicNodeParser,
            backgroundNodeParser,
            pauseNodeParser,
            mainCharacterNodeParser,
            personNodeParser,
            narrationNodeParser,
            tutorialNodeParser,
            setNodeParser,
            sut
        };

        sut.Name.Should().Be(string.Empty);
        sut.NameRegex.Should().Be("(if|while)");
        sut.IsArgumentRequired.Should().BeTrue();
        sut.ElementParser.Should().BeSameAs(elementParser);
        sut.ConditionParser.Should().BeSameAs(conditionParser);
        sut.IfBlockSettings.Should().BeOfType<ElementParserSettings.IfBlock>();
        sut.IfBlockSettings.ChildParsers.Should().BeEquivalentTo(parsers);
        sut.BlockSettings.Should().BeOfType<ElementParserSettings.Block>();
        sut.BlockSettings.ChildParsers.Should().BeEquivalentTo(parsers);
    }

    [Fact]
    public async Task ShouldLogArgumentParsingError()
    {
        var errorMessage = "Erro proposital.";
        var argument = "Gibberish";
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => conditionParser.Parse(argument)).Returns(Result.Fail<ICondition>(errorMessage));
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldBeEmpty();
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ShouldExitOnWhileBlockParsingError()
    {
        A.CallTo(() => documentReader.Command).Returns("while");
        A.CallTo(() => documentReader.Argument).Returns("a = b");
        A.CallTo(() => parsingContext.IsSuccess).Returns(false);

        var condition = A.Dummy<ICondition>();
        A.CallTo(() => conditionParser.Parse("a = b")).Returns(Result.Ok(condition));

        var ifBlockNode = A.Dummy<INode>();
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.BlockSettings))
            .DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldBeEmpty();
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.BlockSettings))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ShouldExitOnIfBlockParsingError()
    {
        A.CallTo(() => documentReader.Command).Returns("if");
        A.CallTo(() => documentReader.Argument).Returns("a = b");
        A.CallTo(() => parsingContext.IsSuccess).Returns(false);

        var condition = A.Dummy<ICondition>();
        A.CallTo(() => conditionParser.Parse("a = b")).Returns(Result.Ok(condition));

        var ifBlockNode = A.Dummy<INode>();
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.IfBlockSettings))
            .DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldBeEmpty();
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.IfBlockSettings))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ShouldExitOnElseBlockParsingError()
    {
        A.CallTo(() => documentReader.Command).ReturnsNextFromSequence("if", "else");
        A.CallTo(() => documentReader.Argument).Returns("a = b");
        A.CallTo(() => parsingContext.IsSuccess).ReturnsNextFromSequence(true, false);
        A.CallTo(() => documentReader.AtEnd).Returns(false);

        var condition = A.Dummy<ICondition>();
        A.CallTo(() => conditionParser.Parse("a = b")).Returns(Result.Ok(condition));

        var ifBlockNode = A.Dummy<INode>();
        var elseBlockNode = A.Dummy<INode>();
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.IfBlockSettings))
            .DoesNothing();
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.BlockSettings))
            .DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldBeEmpty();

        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.IfBlockSettings))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.BlockSettings))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task IfWithElse()
    {
        A.CallTo(() => documentReader.AtEnd).Returns(false);
        A.CallTo(() => documentReader.Command).ReturnsNextFromSequence("if", "else");
        A.CallTo(() => documentReader.Argument).Returns("a = b");
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);

        var condition = A.Dummy<ICondition>();
        A.CallTo(() => conditionParser.Parse("a = b")).Returns(Result.Ok(condition));

        var ifBlockNode = A.Dummy<INode>();
        var elseBlockNode = A.Dummy<INode>();
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.IfBlockSettings))
            .Invokes(i => i.Arguments.Get<IParentParsingContext>(2).AddNode(ifBlockNode));
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.BlockSettings))
            .Invokes(i => i.Arguments.Get<IParentParsingContext>(2).AddNode(elseBlockNode));

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldContainSingle<IfNode>(i =>
        {
            i.Condition.Should().BeSameAs(condition);
            i.IfBlock.ShouldContainOnly(ifBlockNode);
            i.IfBlock.While.Should().BeNull();
            i.ElseBlock.ShouldContainOnly(elseBlockNode);
            i.ElseBlock.While.Should().BeNull();
            i.ChildBlock.Should().BeNull();
        });
    }

    [Fact]
    public async Task IfWithoutElse()
    {
        A.CallTo(() => documentReader.AtEnd).Returns(false);
        A.CallTo(() => documentReader.Command).ReturnsNextFromSequence("if", "end");
        A.CallTo(() => documentReader.Argument).Returns("a = b");
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);

        var condition = A.Dummy<ICondition>();
        A.CallTo(() => conditionParser.Parse("a = b")).Returns(Result.Ok(condition));

        var ifBlockNode = A.Dummy<INode>();
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.IfBlockSettings))
            .Invokes(i => i.Arguments.Get<IParentParsingContext>(2).AddNode(ifBlockNode));

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldContainSingle<IfNode>(i =>
        {
            i.Condition.Should().BeSameAs(condition);
            i.IfBlock.ShouldContainOnly(ifBlockNode);
            i.IfBlock.While.Should().BeNull();
            i.ElseBlock.ShouldBeEmpty();
            i.ElseBlock.While.Should().BeNull();
            i.ChildBlock.Should().BeNull();
        });
    }

    [Fact]
    public async Task IfWithoutElseBecauseEndWasReached()
    {
        A.CallTo(() => documentReader.AtEnd).Returns(true);
        A.CallTo(() => documentReader.Command).Returns("if");
        A.CallTo(() => documentReader.Argument).Returns("a = b");
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);

        var condition = A.Dummy<ICondition>();
        A.CallTo(() => conditionParser.Parse("a = b")).Returns(Result.Ok(condition));

        var ifBlockNode = A.Dummy<INode>();
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.IfBlockSettings))
            .Invokes(i => i.Arguments.Get<IParentParsingContext>(2).AddNode(ifBlockNode));

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldContainSingle<IfNode>(i =>
        {
            i.Condition.Should().BeSameAs(condition);
            i.IfBlock.ShouldContainOnly(ifBlockNode);
            i.IfBlock.While.Should().BeNull();
            i.ElseBlock.ShouldBeEmpty();
            i.ElseBlock.While.Should().BeNull();
            i.ChildBlock.Should().BeNull();
        });
    }

    [Fact]
    public async Task While()
    {
        A.CallTo(() => documentReader.AtEnd).Returns(false);
        A.CallTo(() => documentReader.Command).Returns("while");
        A.CallTo(() => documentReader.Argument).Returns("a = b");
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);

        var condition = A.Dummy<ICondition>();
        A.CallTo(() => conditionParser.Parse("a = b")).Returns(Result.Ok(condition));

        var blockNode = A.Dummy<INode>();
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.BlockSettings))
            .Invokes(i => i.Arguments.Get<IParentParsingContext>(2).AddNode(blockNode));

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldContainSingle<BlockNode>(i =>
        {
            i.ChildBlock.While.Should().BeSameAs(condition);
            i.ChildBlock.ShouldContainOnly(blockNode);
        });
    }

}
