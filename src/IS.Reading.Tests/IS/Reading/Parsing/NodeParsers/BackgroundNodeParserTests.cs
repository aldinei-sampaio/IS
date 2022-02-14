using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.ArgumentParsers;
using IS.Reading.Parsing.NodeParsers.BackgroundParsers;
using IS.Reading.State;

namespace IS.Reading.Parsing.NodeParsers;

public class BackgroundNodeParserTests
{
    private readonly IDocumentReader reader;
    private readonly IParsingContext context;
    private readonly FakeParentParsingContext parentContext = new();
    private readonly IElementParser elementParser;

    private readonly IImageArgumentParser imageArgumentParser;
    private readonly IBackgroundColorNodeParser backgroundColorNodeParser;
    private readonly IBackgroundLeftNodeParser backgroundLeftNodeParser;
    private readonly IBackgroundRightNodeParser backgroundRightNodeParser;
    private readonly IBackgroundScrollNodeParser backgroundScrollNodeParser;
    private readonly IPauseNodeParser pauseNodeParser;
    private readonly BackgroundNodeParser sut;

    public BackgroundNodeParserTests()
    {
        reader = A.Fake<IDocumentReader>(i => i.Strict());
        context = Helper.FakeParsingContext();
        elementParser = A.Fake<IElementParser>(i => i.Strict());

        imageArgumentParser = A.Dummy<IImageArgumentParser>();
        backgroundColorNodeParser = Helper.FakeParser<IBackgroundColorNodeParser>("color");
        backgroundLeftNodeParser = Helper.FakeParser<IBackgroundLeftNodeParser>("left");
        backgroundRightNodeParser = Helper.FakeParser<IBackgroundRightNodeParser>("right");
        backgroundScrollNodeParser = Helper.FakeParser<IBackgroundScrollNodeParser>("scroll");
        pauseNodeParser = Helper.FakeParser<IPauseNodeParser>("pause");

        sut = new(
            elementParser, 
            imageArgumentParser,
            backgroundColorNodeParser,
            backgroundLeftNodeParser,
            backgroundRightNodeParser,
            backgroundScrollNodeParser,
            pauseNodeParser
        );
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("background");
        sut.IsArgumentRequired.Should().BeFalse();
        sut.ElementParser.Should().BeSameAs(elementParser);
        sut.ImageArgumentParser.Should().BeSameAs(imageArgumentParser);

        sut.Settings.Should().BeOfType<ElementParserSettings.Aggregated>()
            .Which.ShouldContainOnly(
                backgroundLeftNodeParser,
                backgroundRightNodeParser,
                backgroundColorNodeParser,
                backgroundScrollNodeParser,
                pauseNodeParser
            );

        sut.DismissNode.Should().BeOfType<BackgroundNode>()
            .Which.State.Should().BeSameAs(BackgroundState.Empty);
    }

    [Fact]
    public async Task ArgumentAndNoAggregation()
    {
        A.CallTo(() => reader.Argument).Returns("gama");
        A.CallTo(() => imageArgumentParser.Parse("gama")).Returns(Result.Ok("gama"));

        A.CallTo(() => context.RegisterDismissNode(sut.DismissNode)).DoesNothing();
        A.CallTo(() => context.IsSuccess).Returns(true);
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings)).DoesNothing();

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.ShouldContainSingle<BlockNode>(i => {
            i.ChildBlock.ShouldContain(
                i => i.Should().BeOfType<BackgroundNode>().Which.ShouldSatisfy(j =>
                {
                    j.Should().NotBeNull();
                    j.State.Should().BeEquivalentTo(new
                    {
                        Name = "gama",
                        Type = BackgroundType.Image,
                        Position = BackgroundPosition.Left
                    });
                }),
                i => i.Should().BeOfType<ScrollNode>()
            );
        });
    }

    [Fact]
    public async Task AggregationAndNoArgument()
    {
        A.CallTo(() => reader.Argument).Returns(string.Empty);

        var parsedNode = A.Dummy<INode>();

        A.CallTo(() => context.RegisterDismissNode(sut.DismissNode)).DoesNothing();
        A.CallTo(() => context.IsSuccess).Returns(true);
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i =>
            {
                var ctx = i.Arguments.Get<IParentParsingContext>(2);
                ctx.AddNode(parsedNode);
            });

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.Nodes.Should().ContainSingle()
            .Which.Should().BeOfType<BlockNode>()
            .Which.ChildBlock.ShouldContainOnly(parsedNode);
    }

    [Fact]
    public async Task DismissNodeMustBeRegistered()
    {
        A.CallTo(() => reader.Argument).Returns(string.Empty);

        var parsedNode = A.Dummy<INode>();
        A.CallTo(() => context.RegisterDismissNode(sut.DismissNode)).DoesNothing();
        A.CallTo(() => context.IsSuccess).Returns(true);
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.GetArgument<IParentParsingContext>(2).AddNode(parsedNode));

        await sut.ParseAsync(reader, context, parentContext);

        A.CallTo(() => context.RegisterDismissNode(sut.DismissNode)).MustHaveHappenedOnceExactly();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ArgumentAndAggregation()
    {
        A.CallTo(() => reader.Argument).Returns("gama");
        A.CallTo(() => imageArgumentParser.Parse("gama")).Returns(Result.Ok("gama"));

        A.CallTo(() => context.RegisterDismissNode(sut.DismissNode)).DoesNothing();
        A.CallTo(() => context.IsSuccess).Returns(true);

        var parsedNode = A.Dummy<INode>();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.GetArgument<IParentParsingContext>(2).AddNode(parsedNode));

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.ShouldContainSingle<BlockNode>(i => {
            i.ChildBlock.ShouldContain(
                i => i.Should().BeOfType<BackgroundNode>().Which.ShouldSatisfy(j =>
                {
                    j.Should().NotBeNull();
                    j.State.Should().BeEquivalentTo(new
                    {
                        Name = "gama",
                        Type = BackgroundType.Image,
                        Position = BackgroundPosition.Left
                    });
                }),
                i => i.Should().BeOfType<ScrollNode>(),
                i => i.Should().BeSameAs(parsedNode)
            );
        });
    }

    [Fact]
    public async Task ShouldLogImageParserErrors()
    {
        var errorMessage = "Erro proposital.";
        var argument = "omega";

        A.CallTo(() => context.LogError(reader, errorMessage)).DoesNothing();
        A.CallTo(() => reader.Argument).Returns(argument);
        A.CallTo(() => imageArgumentParser.Parse(argument)).Returns(Result.Fail<string>(errorMessage));

        await sut.ParseAsync(reader, context, parentContext);

        A.CallTo(() => context.LogError(reader, errorMessage)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task NoArgumentAndNoAggregation()
    {
        var errorMessage = "O comando 'background' espera um parâmetro ou um ou mais comandos adicionais.";

        A.CallTo(() => context.LogError(reader, errorMessage)).DoesNothing();
        A.CallTo(() => reader.Argument).Returns(string.Empty);
        A.CallTo(() => context.IsSuccess).Returns(true);
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings)).DoesNothing();

        await sut.ParseAsync(reader, context, parentContext);

        A.CallTo(() => context.LogError(reader, errorMessage)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ExitOnElementParserError()
    {
        A.CallTo(() => reader.Argument).Returns(string.Empty);
        A.CallTo(() => context.IsSuccess).Returns(false);
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings)).DoesNothing();

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.ShouldBeEmpty();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings)).MustHaveHappenedOnceExactly();
    }
}
