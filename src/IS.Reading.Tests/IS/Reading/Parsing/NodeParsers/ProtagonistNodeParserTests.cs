using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.AttributeParsers;
using IS.Reading.Parsing.TextParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers;

public class ProtagonistNodeParserTests
{
    private readonly IElementParser elementParser;
    private readonly IWhenAttributeParser whenAttributeParser;
    private readonly INameTextParser nameTextParser;
    private readonly ProtagonistNodeParser sut;

    public ProtagonistNodeParserTests()
    {
        elementParser = A.Fake<IElementParser>(i => i.Strict());
        whenAttributeParser = Helper.FakeParser<IWhenAttributeParser>("when");        
        nameTextParser = A.Dummy<INameTextParser>();
        sut = new(elementParser, whenAttributeParser, nameTextParser);
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("protagonist");
        sut.Settings.TextParser.Should().BeSameAs(nameTextParser);
        sut.Settings.AttributeParsers["when"].Should().BeSameAs(whenAttributeParser);
        sut.Settings.AttributeParsers.Count.Should().Be(1);
        sut.Settings.ChildParsers.Count.Should().Be(0);
    }

    [Theory]
    [InlineData("chamusca", "chamusca")]
    [InlineData("", null)]
    public async Task ParseAsyncShouldReturnProtagonistNode(string parsedValue, string protagonistName)
    {
        var reader = A.Dummy<XmlReader>();
        var context = A.Dummy<IParsingContext>();
        var parentContext = new FakeParentParsingContext();

        var when = A.Dummy<ICondition>();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i =>
            {
                var ctx = i.GetArgument<IParentParsingContext>(2);
                ctx.ParsedText = parsedValue;
                ctx.When = when;
            });

        await sut.ParseAsync(reader, context, parentContext);

        var node = parentContext.ShouldContainSingle<ProtagonistNode>();
        node.ProtagonistName.Should().Be(protagonistName);
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
    public void DismissNodeShouldClearProtagonist()
    {
        var node = sut.DismissNode;
        var dismiss = node.Should().BeOfType<DismissNode<ProtagonistNode>>().Which;
        var protagNode = dismiss.ChangeNode.Should().BeOfType<ProtagonistNode>().Which;
        protagNode.ProtagonistName.Should().BeNull();
        protagNode.When.Should().BeNull();
    }
}
