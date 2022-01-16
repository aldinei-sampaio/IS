using IS.Reading.Conditions;
using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.AttributeParsers;
using IS.Reading.Parsing.NodeParsers.BackgroundParsers;
using IS.Reading.Parsing.TextParsers;
using IS.Reading.State;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers;

public class BackgroundNodeParserTests
{
    private readonly XmlReader reader;
    private readonly IParsingContext context;
    private readonly FakeParentParsingContext parentContext = new();
    private readonly IElementParser elementParser;

    private readonly IWhenAttributeParser whenAttributeParser;
    private readonly IBackgroundImageTextParser backgroundImageTextParser;
    private readonly IBackgroundColorNodeParser backgroundColorNodeParser;
    private readonly IBackgroundLeftNodeParser backgroundLeftNodeParser;
    private readonly IBackgroundRightNodeParser backgroundRightNodeParser;
    private readonly IBackgroundScrollNodeParser backgroundScrollNodeParser;
    private readonly IPauseNodeParser pauseNodeParser;
    private readonly BackgroundNodeParser sut;

    public BackgroundNodeParserTests()
    {
        reader = A.Dummy<XmlReader>();
        context = Helper.FakeParsingContext();
        elementParser = A.Fake<IElementParser>(i => i.Strict());

        backgroundImageTextParser = A.Dummy<IBackgroundImageTextParser>();
        whenAttributeParser = Helper.FakeParser<IWhenAttributeParser>("when");
        backgroundColorNodeParser = Helper.FakeParser<IBackgroundColorNodeParser>("color");
        backgroundLeftNodeParser = Helper.FakeParser<IBackgroundLeftNodeParser>("left");
        backgroundRightNodeParser = Helper.FakeParser<IBackgroundRightNodeParser>("right");
        backgroundScrollNodeParser = Helper.FakeParser<IBackgroundScrollNodeParser>("scroll");
        pauseNodeParser = Helper.FakeParser<IPauseNodeParser>("pause");

        sut = new(
            elementParser, 
            whenAttributeParser, 
            backgroundImageTextParser,
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
        sut.Settings.ShouldBeNormal(
            whenAttributeParser,
            backgroundImageTextParser,
            backgroundLeftNodeParser,
            backgroundRightNodeParser,
            backgroundColorNodeParser,
            backgroundScrollNodeParser,
            pauseNodeParser
        );

        sut.DismissNode.Should().BeOfType<DismissNode<BackgroundNode>>()
            .Which.ChangeNode.Should().BeEquivalentTo(new
            {
                State = BackgroundState.Empty,
                When = (ICondition)null
            });
    }

    [Fact]
    public async Task SuccessText()
    {
        A.CallTo(() => context.RegisterDismissNode(sut.DismissNode)).DoesNothing();
        var when = A.Dummy<ICondition>();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i =>
            {
                var ctx = i.Arguments.Get<IParentParsingContext>(2);
                ctx.ParsedText = "gama";
                ctx.When = when;
            });

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.ShouldContainSingle<BlockNode>(i => {
            i.When.Should().BeSameAs(when);
            i.While.Should().BeNull();
            i.ChildBlock.ShouldContain(
                i => i.Should().BeOfType<BackgroundNode>().Which.ShouldSatisfy(j =>
                {
                    j.Should().NotBeNull();
                    j.When.Should().BeNull();
                    j.State.Should().BeEquivalentTo(new
                    {
                        Name = "gama",
                        Type = BackgroundType.Image,
                        Position = BackgroundPosition.Left
                    });
                }),
                i => i.Should().BeOfType<ScrollNode>().Which.When.Should().BeNull()
            );
        });
    }

    [Fact]
    public async Task SuccessElement()
    {
        var parsedNode = A.Dummy<INode>();
        var when = A.Dummy<ICondition>();

        A.CallTo(() => context.RegisterDismissNode(sut.DismissNode)).DoesNothing();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i =>
            {
                var ctx = i.Arguments.Get<IParentParsingContext>(2);
                ctx.AddNode(parsedNode);
                ctx.When = when;
            });

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.Nodes.Should().ContainSingle()
            .Which.Should().BeOfType<BlockNode>()
            .Which.ShouldSatisfy(i =>
            {
                i.When.Should().BeSameAs(when);
                i.While.Should().BeNull();
                i.ChildBlock.Nodes.Should().ContainSingle().Which.Should().BeSameAs(parsedNode);
            });
    }

    [Fact]
    public async Task DismissNodeMustBeRegistered()
    {
        var parsedNode = A.Dummy<INode>();

        A.CallTo(() => context.RegisterDismissNode(sut.DismissNode)).DoesNothing();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.GetArgument<IParentParsingContext>(2).AddNode(parsedNode));

        await sut.ParseAsync(reader, context, parentContext);

        A.CallTo(() => context.RegisterDismissNode(sut.DismissNode)).MustHaveHappenedOnceExactly();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task NullTextEmptyBlock()
    {
        const string message = "Nome de imagem ou elemento filho era esperado.";
        A.CallTo(() => context.LogError(reader, message)).DoesNothing();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings)).DoesNothing();

        await sut.ParseAsync(reader, context, parentContext);
        parentContext.ShouldBeEmpty();

        A.CallTo(() => context.LogError(reader, message)).MustHaveHappenedOnceExactly();
    }
}
