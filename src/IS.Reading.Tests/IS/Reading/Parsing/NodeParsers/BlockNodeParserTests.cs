using IS.Reading.Conditions;
using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.ConditionParsers;

namespace IS.Reading.Parsing.NodeParsers;

public class BlockNodeParserTests
{
    private readonly IDocumentReader reader;
    private readonly IParsingContext context;
    private readonly FakeParentParsingContext parentContext = new();
    private readonly IElementParser elementParser;
    private readonly IMusicNodeParser musicNodeParser;
    private readonly IBackgroundNodeParser backgroundNodeParser;
    private readonly IPauseNodeParser pauseNodeParser;
    private readonly IProtagonistNodeParser protagonistNodeParser;
    private readonly IPersonNodeParser personNodeParser;
    private readonly INarrationNodeParser narrationNodeParser;
    private readonly ITutorialNodeParser tutorialNodeParser;
    private readonly ISetNodeParser setNodeParser;
    private readonly IConditionParser conditionParser;
    private readonly BlockNodeParser sut;

    public BlockNodeParserTests()
    {
        reader = A.Dummy<IDocumentReader>();

        context = Helper.FakeParsingContext();
        elementParser = A.Fake<IElementParser>(i => i.Strict(StrictFakeOptions.AllowToString));
        musicNodeParser = Helper.FakeParser<IMusicNodeParser>("music");
        backgroundNodeParser = Helper.FakeParser<IBackgroundNodeParser>("background");
        pauseNodeParser = Helper.FakeParser<IPauseNodeParser>("pause");
        protagonistNodeParser = Helper.FakeParser<IProtagonistNodeParser>("protagonist");
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
            protagonistNodeParser,
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
            protagonistNodeParser,
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
    public async Task IfWithElse()
    {
        A.CallTo(() => reader.AtEnd).Returns(false);
        A.CallTo(() => reader.Command).ReturnsNextFromSequence("if", "else");
        A.CallTo(() => reader.Argument).Returns("a = b");
        A.CallTo(() => context.IsSuccess).Returns(true);

        var condition = A.Dummy<ICondition>();
        A.CallTo(() => conditionParser.Parse("a = b")).Returns(Result.Ok(condition));

        var ifBlockNode = A.Dummy<INode>();
        var elseBlockNode = A.Dummy<INode>();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.IfBlockSettings))
            .Invokes(i => i.Arguments.Get<IParentParsingContext>(2).AddNode(ifBlockNode));
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.BlockSettings))
            .Invokes(i => i.Arguments.Get<IParentParsingContext>(2).AddNode(elseBlockNode));

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.ShouldContainSingle<IfNode>(i =>
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
        A.CallTo(() => reader.AtEnd).Returns(false);
        A.CallTo(() => reader.Command).ReturnsNextFromSequence("if", "end");
        A.CallTo(() => reader.Argument).Returns("a = b");
        A.CallTo(() => context.IsSuccess).Returns(true);

        var condition = A.Dummy<ICondition>();
        A.CallTo(() => conditionParser.Parse("a = b")).Returns(Result.Ok(condition));

        var ifBlockNode = A.Dummy<INode>();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.IfBlockSettings))
            .Invokes(i => i.Arguments.Get<IParentParsingContext>(2).AddNode(ifBlockNode));

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.ShouldContainSingle<IfNode>(i =>
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
        A.CallTo(() => reader.AtEnd).Returns(false);
        A.CallTo(() => reader.Command).Returns("while");
        A.CallTo(() => reader.Argument).Returns("a = b");
        A.CallTo(() => context.IsSuccess).Returns(true);

        var condition = A.Dummy<ICondition>();
        A.CallTo(() => conditionParser.Parse("a = b")).Returns(Result.Ok(condition));

        var blockNode = A.Dummy<INode>();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.BlockSettings))
            .Invokes(i => i.Arguments.Get<IParentParsingContext>(2).AddNode(blockNode));

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.ShouldContainSingle<BlockNode>(i =>
        {
            i.ChildBlock.While.Should().BeSameAs(condition);
            i.ChildBlock.ShouldContainOnly(blockNode);
        });
    }

}
