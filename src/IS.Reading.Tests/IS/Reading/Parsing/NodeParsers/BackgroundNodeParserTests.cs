using IS.Reading.Nodes;
using IS.Reading.Parsing.AttributeParsers;
using IS.Reading.Parsing.TextParsers;
using IS.Reading.State;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers;

public class BackgroundNodeParserTests
{
    private readonly XmlReader reader;
    private readonly IParsingContext context;
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
        context = A.Fake<IParsingContext>(i => i.Strict());
        elementParser = A.Fake<IElementParser>(i => i.Strict());

        backgroundImageTextParser = A.Dummy<IBackgroundImageTextParser>();
        whenAttributeParser = A.Dummy<IWhenAttributeParser>();
        A.CallTo(() => whenAttributeParser.AttributeName).Returns("when");
        backgroundColorNodeParser = A.Dummy<IBackgroundColorNodeParser>();
        A.CallTo(() => backgroundColorNodeParser.ElementName).Returns("color");
        backgroundLeftNodeParser = A.Dummy<IBackgroundLeftNodeParser>();
        A.CallTo(() => backgroundLeftNodeParser.ElementName).Returns("left");
        backgroundRightNodeParser = A.Dummy<IBackgroundRightNodeParser>();
        A.CallTo(() => backgroundRightNodeParser.ElementName).Returns("right");
        backgroundScrollNodeParser = A.Dummy<IBackgroundScrollNodeParser>();
        A.CallTo(() => backgroundScrollNodeParser.ElementName).Returns("scroll");
        pauseNodeParser = A.Dummy<IPauseNodeParser>();
        A.CallTo(() => pauseNodeParser.ElementName).Returns("pause");

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
        sut.ElementName.Should().Be("background");
        sut.Settings.AttributeParsers.Should().ContainValues(whenAttributeParser);
        sut.Settings.ChildParsers["left"].Should().BeSameAs(backgroundLeftNodeParser);
        sut.Settings.ChildParsers["right"].Should().BeSameAs(backgroundRightNodeParser);
        sut.Settings.ChildParsers["color"].Should().BeSameAs(backgroundColorNodeParser);
        sut.Settings.ChildParsers["scroll"].Should().BeSameAs(backgroundScrollNodeParser);
        sut.Settings.ChildParsers["pause"].Should().BeSameAs(pauseNodeParser);
        sut.Settings.ChildParsers.Should().HaveCount(5);
        sut.Settings.TextParser.Should().BeSameAs(backgroundImageTextParser);
    }

    [Fact]
    public async Task SuccessText()
    {
        var parsed = A.Dummy<IElementParsedData>();
        A.CallTo(() => parsed.Text).Returns("alfa");
        
        A.CallTo(() => elementParser.ParseAsync(reader, context, sut.Settings)).Returns(parsed);

        var result = await sut.ParseAsync(reader, context);

        result.Should().NotBeNull();
        result.Should().BeOfType<BlockNode>();

        var node = (BlockNode)result;
        node.When.Should().BeSameAs(parsed.When);
        node.While.Should().BeSameAs(parsed.While);
        node.ChildBlock.Should().NotBeNull();
        node.ChildBlock.ForwardQueue.Count.Should().Be(2);
        {
            var subnode = node.ChildBlock.ForwardQueue.Dequeue() as BackgroundChangeNode;
            subnode.Should().NotBeNull();
            subnode.State.Name.Should().Be("alfa");
            subnode.State.Type.Should().Be(BackgroundType.Image);
            subnode.State.Position.Should().Be(BackgroundPosition.Left);
            subnode.When.Should().BeNull();
        }
        {
            var subnode = node.ChildBlock.ForwardQueue.Dequeue() as BackgroundScrollNode;
            subnode.Should().NotBeNull();
            subnode.When.Should().BeNull();
        }
    }

    [Fact]
    public async Task SuccessElement()
    {
        var block = new Navigation.Block();
        block.ForwardQueue.Enqueue(A.Dummy<Navigation.INode>());

        var parsed = A.Dummy<IElementParsedData>();
        A.CallTo(() => parsed.Text).Returns(null);
        A.CallTo(() => parsed.Block).Returns(block);

        A.CallTo(() => elementParser.ParseAsync(reader, context, sut.Settings)).Returns(parsed);

        var result = await sut.ParseAsync(reader, context);

        result.Should().NotBeNull();
        result.Should().BeOfType<BlockNode>();

        var node = (BlockNode)result;
        node.When.Should().BeSameAs(parsed.When);
        node.While.Should().BeSameAs(parsed.While);
        node.ChildBlock.Should().BeSameAs(block);
    }

    [Fact]
    public async Task NullTextEmptyBlock()
    {
        const string message = "Nome de imagem ou elemento filho era esperado.";
        A.CallTo(() => context.LogError(reader, message)).DoesNothing();

        var parsed = A.Dummy<IElementParsedData>();
        A.CallTo(() => parsed.Text).Returns(null);

        A.CallTo(() => elementParser.ParseAsync(reader, context, sut.Settings)).Returns(parsed);

        var result = await sut.ParseAsync(reader, context);

        result.Should().BeNull();
        A.CallTo(() => context.LogError(reader, message)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task NullTextNullBlock()
    {
        const string message = "Nome de imagem ou elemento filho era esperado.";
        A.CallTo(() => context.LogError(reader, message)).DoesNothing();

        var parsed = A.Dummy<IElementParsedData>();
        A.CallTo(() => parsed.Text).Returns(null);
        A.CallTo(() => parsed.Block).Returns(null);

        A.CallTo(() => elementParser.ParseAsync(reader, context, sut.Settings)).Returns(parsed);

        var result = await sut.ParseAsync(reader, context);

        result.Should().BeNull();
        A.CallTo(() => context.LogError(reader, message)).MustHaveHappenedOnceExactly();
    }
}
