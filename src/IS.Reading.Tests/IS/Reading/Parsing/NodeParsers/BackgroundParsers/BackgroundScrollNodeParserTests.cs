using IS.Reading.Conditions;
using IS.Reading.Nodes;
using IS.Reading.Parsing.AttributeParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.BackgroundParsers;

public class BackgroundScrollNodeParserTests
{
    private readonly XmlReader reader;
    private readonly IParsingContext context;
    private readonly FakeParentParsingContext parentContext = new();
    private readonly IElementParser elementParser;
    private readonly IWhenAttributeParser whenAttributeParser;
    private readonly BackgroundScrollNodeParser sut;

    public BackgroundScrollNodeParserTests()
    {
        reader = A.Dummy<XmlReader>();
        context = A.Fake<IParsingContext>(i => i.Strict());
        elementParser = A.Fake<IElementParser>(i => i.Strict());
        whenAttributeParser = Helper.FakeParser<IWhenAttributeParser>("when");

        sut = new(elementParser, whenAttributeParser);
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("scroll");
        sut.Settings.AttributeParsers["when"].Should().BeSameAs(whenAttributeParser);
        sut.Settings.AttributeParsers.Count.Should().Be(1);
        sut.Settings.ChildParsers.Count.Should().Be(0);
        sut.Settings.TextParser.Should().BeNull();
    }

    [Fact]
    public async Task Success()
    {
        var when = A.Dummy<ICondition>();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.Arguments.Get<IParentParsingContext>(2).When = when);

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.ShouldContainSingle<ScrollNode>(i => i.When.Should().BeSameAs(when));
    }
}
