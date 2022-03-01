using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.ArgumentParsers;
using IS.Reading.Parsing.NodeParsers.PersonParsers;

namespace IS.Reading.Parsing.NodeParsers;

public class PersonNodeParserTests
{
    private readonly IDocumentReader documentReader;
    private readonly IParsingContext parsingContext;
    private readonly FakeParentParsingContext parentParsingContext;
    private readonly IParsingSceneContext parsingSceneContext;

    private readonly IElementParser elementParser;
    private readonly INameArgumentParser nameArgumentParser;
    private readonly ISpeechNodeParser speechNodeParser;
    private readonly IThoughtNodeParser thoughtNodeParser;
    private readonly IMoodNodeParser moodNodeParser;
    private readonly IPauseNodeParser pauseNodeParser;
    private readonly ISetNodeParser setNodeParser;
    private readonly ITitleNodeParser titleNodeParser;
    private readonly PersonNodeParser sut;
    
    public PersonNodeParserTests()
    {
        elementParser = A.Fake<IElementParser>(i => i.Strict());
        nameArgumentParser = A.Fake<INameArgumentParser>(i => i.Strict());
        speechNodeParser = Helper.FakeParser<ISpeechNodeParser>("speech");
        thoughtNodeParser = Helper.FakeParser<IThoughtNodeParser>("thought");
        moodNodeParser = Helper.FakeParser<IMoodNodeParser>("mood");
        pauseNodeParser = Helper.FakeParser<IPauseNodeParser>("pause");
        setNodeParser = Helper.FakeParser<ISetNodeParser>("set");
        titleNodeParser = Helper.FakeParser<ITitleNodeParser>("title");
        sut = new(
            elementParser, 
            nameArgumentParser,
            speechNodeParser, 
            thoughtNodeParser, 
            moodNodeParser, 
            pauseNodeParser,
            setNodeParser,
            titleNodeParser
        );

        documentReader = A.Fake<IDocumentReader>(i => i.Strict());
        parsingContext = Helper.FakeParsingContext();
        parentParsingContext = new FakeParentParsingContext();
        parsingSceneContext = A.Fake<IParsingSceneContext>(i => i.Strict());
        A.CallTo(() => parsingContext.SceneContext).Returns(parsingSceneContext);
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("@");
        sut.IsArgumentRequired.Should().BeTrue();
        sut.ElementParser.Should().BeSameAs(elementParser);
        sut.NameArgumentParser.Should().BeSameAs(nameArgumentParser);
        sut.Settings.Should().BeOfType<ElementParserSettings.Aggregated>();
        sut.Settings.ChildParsers.Should().BeEquivalentTo(
            speechNodeParser, 
            thoughtNodeParser, 
            moodNodeParser, 
            pauseNodeParser, 
            setNodeParser,
            titleNodeParser
        );
        sut.InitializeMoodNode.Should().BeOfType<MoodNode>().Which.MoodType.Should().Be(MoodType.Normal);
        sut.DismissMoodNode.Should().BeOfType<MoodNode>().Which.MoodType.Should().BeNull();
    }

    [Fact]
    public async Task ShouldLogArgumentParserError()
    {
        var errorMessage = "Erro proposital.";
        var argument = "alabama";
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => nameArgumentParser.Parse(argument)).Returns(Result.Fail<string>(errorMessage));
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldBeEmpty();
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ShouldExitOnEventParserError()
    {
        var argument = "Gibberish";

        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => nameArgumentParser.Parse(argument)).Returns(Result.Ok(argument));
        A.CallTo(() => parsingContext.IsSuccess).Returns(false);
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.Settings))
            .DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldBeEmpty();
    }

    [Fact]
    public async Task ShouldExitWhenNoChildNodeIsParsed()
    {
        var argument = "slacksucks";

        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => nameArgumentParser.Parse(argument)).Returns(Result.Ok(argument));
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.Settings))
            .DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldBeEmpty();
    }

    [Fact]
    public async Task ParsingSuccess()
    {
        var argument = "joe";

        var dummyNode = A.Dummy<INode>();

        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => nameArgumentParser.Parse(argument)).Returns(Result.Ok(argument));
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.GetArgument<IParentParsingContext>(2).AddNode(dummyNode));

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldContainSingle<PersonNode>(
            i => i.ChildBlock.ShouldContain(
                i => i.Should().BeSameAs(sut.InitializeMoodNode),
                i => i.Should().BeOfType<BalloonTitleNode>().Which.TextSource.ToString().Should().Be("{joe}"),
                i => i.Should().BeSameAs(dummyNode),
                i => i.Should().BeSameAs(sut.DismissTitleNode),
                i => i.Should().BeSameAs(sut.DismissMoodNode)
            )
        );
    }

    [Fact]
    public async Task ShouldInsertInitTitleNodeWhenBlockContainsTitleNodeAfterPauseNode()
    {
        var argument = "joe";

        var dummyPauseNode = A.Dummy<IPauseNode>();
        var dummyTitleNode = new BalloonTitleNode(null);

        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => nameArgumentParser.Parse(argument)).Returns(Result.Ok(argument));
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => {
                var ctx = i.GetArgument<IParentParsingContext>(2);
                ctx.AddNode(dummyPauseNode);
                ctx.AddNode(dummyTitleNode);
            });

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldContainSingle<PersonNode>(
            i => i.ChildBlock.ShouldContain(
                i => i.Should().BeSameAs(sut.InitializeMoodNode),
                i => i.Should().BeOfType<BalloonTitleNode>().Which.TextSource.ToString().Should().Be("{joe}"),
                i => i.Should().BeSameAs(dummyPauseNode),
                i => i.Should().BeSameAs(dummyTitleNode),
                i => i.Should().BeSameAs(sut.DismissTitleNode),
                i => i.Should().BeSameAs(sut.DismissMoodNode)
            )
        );
    }

    [Fact]
    public async Task ShouldInsertInitTitleNodeWhenBlockContainsTitleNodeAfterNodeWithChildren()
    {
        var argument = "joe";

        var dummyNodeWithChildren = A.Fake<INode>();
        A.CallTo(() => dummyNodeWithChildren.ChildBlock).Returns(A.Dummy<IBlock>());
        var dummyTitleNode = new BalloonTitleNode(null);

        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => nameArgumentParser.Parse(argument)).Returns(Result.Ok(argument));
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => {
                var ctx = i.GetArgument<IParentParsingContext>(2);
                ctx.AddNode(dummyNodeWithChildren);
                ctx.AddNode(dummyTitleNode);
            });

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldContainSingle<PersonNode>(
            i => i.ChildBlock.ShouldContain(
                i => i.Should().BeSameAs(sut.InitializeMoodNode),
                i => i.Should().BeOfType<BalloonTitleNode>().Which.TextSource.ToString().Should().Be("{joe}"),
                i => i.Should().BeSameAs(dummyNodeWithChildren),
                i => i.Should().BeSameAs(dummyTitleNode),
                i => i.Should().BeSameAs(sut.DismissTitleNode),
                i => i.Should().BeSameAs(sut.DismissMoodNode)
            )
        );
    }

    [Fact]
    public async Task ShouldNotInsertInitTitleNodeWhenBlockContainsTitleNode()
    {
        var argument = "joe";

        var dummyTitleNode = new BalloonTitleNode(null);

        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => nameArgumentParser.Parse(argument)).Returns(Result.Ok(argument));
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.GetArgument<IParentParsingContext>(2).AddNode(dummyTitleNode));

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldContainSingle<PersonNode>(
            i => i.ChildBlock.ShouldContain(
                i => i.Should().BeSameAs(sut.InitializeMoodNode),
                i => i.Should().BeSameAs(dummyTitleNode),
                i => i.Should().BeSameAs(sut.DismissTitleNode),
                i => i.Should().BeSameAs(sut.DismissMoodNode)
            )
        );
    }

    [Fact]
    public async Task ShouldNotInsertInitMoodNodeWhenBlockContainsMoodNode()
    {
        var argument = "joe";

        var dummyMoodNode = new MoodNode(null);

        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => nameArgumentParser.Parse(argument)).Returns(Result.Ok(argument));
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.GetArgument<IParentParsingContext>(2).AddNode(dummyMoodNode));

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldContainSingle<PersonNode>(
            i => i.ChildBlock.ShouldContain(
                i => i.Should().BeOfType<BalloonTitleNode>().Which.TextSource.ToString().Should().Be("{joe}"),
                i => i.Should().BeSameAs(dummyMoodNode),
                i => i.Should().BeSameAs(sut.DismissTitleNode),
                i => i.Should().BeSameAs(sut.DismissMoodNode)
            )
        );
    }

    [Fact]
    public async Task ShouldInsertInitMoodNodeWhenBlockContaisMoodNodeAfterPauseNode()
    {
        var argument = "joe";

        var dummyPauseNode = A.Dummy<IPauseNode>();
        var dummyMoodNode = new MoodNode(null);

        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => nameArgumentParser.Parse(argument)).Returns(Result.Ok(argument));
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => {
                var ctx = i.GetArgument<IParentParsingContext>(2);
                ctx.AddNode(dummyPauseNode);
                ctx.AddNode(dummyMoodNode);
            });

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldContainSingle<PersonNode>(
            i => i.ChildBlock.ShouldContain(
                i => i.Should().BeSameAs(sut.InitializeMoodNode),
                i => i.Should().BeOfType<BalloonTitleNode>().Which.TextSource.ToString().Should().Be("{joe}"),
                i => i.Should().BeSameAs(dummyPauseNode),
                i => i.Should().BeSameAs(dummyMoodNode),
                i => i.Should().BeSameAs(sut.DismissTitleNode),
                i => i.Should().BeSameAs(sut.DismissMoodNode)
            )
        );
    }

    [Fact]
    public async Task ShouldInsertInitMoodNodeWhenBlockContaisMoodNodeAfterNodeWithChildren()
    {
        var argument = "joe";

        var dummyNodeWithChildren = A.Fake<INode>();
        A.CallTo(() => dummyNodeWithChildren.ChildBlock).Returns(A.Dummy<IBlock>());
        var dummyMoodNode = new MoodNode(null);

        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => nameArgumentParser.Parse(argument)).Returns(Result.Ok(argument));
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => {
                var ctx = i.GetArgument<IParentParsingContext>(2);
                ctx.AddNode(dummyNodeWithChildren);
                ctx.AddNode(dummyMoodNode);
            });

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldContainSingle<PersonNode>(
            i => i.ChildBlock.ShouldContain(
                i => i.Should().BeSameAs(sut.InitializeMoodNode),
                i => i.Should().BeOfType<BalloonTitleNode>().Which.TextSource.ToString().Should().Be("{joe}"),
                i => i.Should().BeSameAs(dummyNodeWithChildren),
                i => i.Should().BeSameAs(dummyMoodNode),
                i => i.Should().BeSameAs(sut.DismissTitleNode),
                i => i.Should().BeSameAs(sut.DismissMoodNode)
            )
        );
    }
}
