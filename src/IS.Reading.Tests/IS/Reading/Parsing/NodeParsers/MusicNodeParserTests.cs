using IS.Reading.Conditions;
using IS.Reading.Nodes;
using IS.Reading.Parsing.AttributeParsers;
using IS.Reading.Parsing.TextParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers;

public class MusicNodeParserTests
{
    private readonly IElementParser elementParser;
    private readonly IWhenAttributeParser whenAttributeParser;
    private readonly INameTextParser nameTextParser;
    private readonly MusicNodeParser sut;

    public MusicNodeParserTests()
    {
        elementParser = A.Fake<IElementParser>(i => i.Strict());
        whenAttributeParser = Helper.FakeParser<IWhenAttributeParser>("when");        
        nameTextParser = A.Dummy<INameTextParser>();
        sut = new(elementParser, whenAttributeParser, nameTextParser);
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("music");
        sut.Settings.TextParser.Should().BeSameAs(nameTextParser);
        sut.Settings.AttributeParsers["when"].Should().BeSameAs(whenAttributeParser);
        sut.Settings.AttributeParsers.Count.Should().Be(1);
        sut.Settings.ChildParsers.Count.Should().Be(0);
    }

    [Theory]
    [InlineData("open_sky", "open_sky")]
    [InlineData("", null)]
    public async Task ParseAsyncShouldReturnMusicNode(string parsedValue, string musicName)
    {
        var reader = A.Dummy<XmlReader>();
        var context = A.Dummy<IParsingContext>();
        var parentContext = new FakeParentParsingContext();

        var when = A.Dummy<ICondition>();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i =>
            {
                var ctx = i.Arguments.Get<IParentParsingContext>(2);
                ctx.ParsedText = parsedValue;
                ctx.When = when;
            });

        await sut.ParseAsync(reader, context, parentContext);

        var node = parentContext.Nodes.Should().ContainSingle()
            .Which.Should().BeOfType<MusicNode>().Which;            

        node.MusicName.Should().Be(musicName);
        node.When.Should().BeSameAs(when);

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ParseAsyncShouldReturnNullIfParsedTextIsNull()
    {
        var reader = A.Dummy<XmlReader>();
        var context = A.Dummy<IParsingContext>();
        var parentContext = new FakeParentParsingContext();

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings)).DoesNothing();

        await sut.ParseAsync(reader, context, parentContext);
        parentContext.ShouldBeEmpty();
    }

    [Fact]
    public void DismissNodeShouldClearMusic()
    {
        var node = sut.DismissNode;
        var dismiss = node.Should().BeOfType<DismissNode<MusicNode>>().Which;
        var protagNode = dismiss.ChangeNode.Should().BeOfType<MusicNode>().Which;
        protagNode.MusicName.Should().BeNull();
        protagNode.When.Should().BeNull();
    }

    [Fact]
    public async Task ShouldSetHasMusic()
    {
        var sceneContext = A.Fake<IParsingSceneContext>(i => i.Strict());
        A.CallTo(() => sceneContext.HasMusic).Returns(false);
        A.CallToSet(() => sceneContext.HasMusic).To(true).DoesNothing();

        var context = A.Fake<IParsingContext>(i => i.Strict());
        A.CallTo(() => context.SceneContext).Returns(sceneContext);
        A.CallTo(() => context.RegisterDismissNode(sut.DismissNode)).DoesNothing();

        var reader = A.Dummy<XmlReader>();
        var parentContext = new FakeParentParsingContext();

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.Arguments.Get<IParentParsingContext>(2).ParsedText = "chaos");

        await sut.ParseAsync(reader, context, parentContext);

        A.CallToSet(() => sceneContext.HasMusic).To(true).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ShouldLogErrorWhenHasMusicIsTrue()
    {
        var message = "Mais de uma definição de música para a mesma cena.";

        var reader = A.Dummy<XmlReader>();
        var parentContext = new FakeParentParsingContext();

        var sceneContext = A.Fake<IParsingSceneContext>(i => i.Strict());
        A.CallTo(() => sceneContext.HasMusic).Returns(true);

        var context = A.Fake<IParsingContext>(i => i.Strict());
        A.CallTo(() => context.SceneContext).Returns(sceneContext);
        A.CallTo(() => context.LogError(reader, message)).DoesNothing();

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.Arguments.Get<IParentParsingContext>(2).ParsedText = "chaos");

        await sut.ParseAsync(reader, context, parentContext);

        A.CallTo(() => context.LogError(reader, message)).MustHaveHappenedOnceExactly();
    }
}
