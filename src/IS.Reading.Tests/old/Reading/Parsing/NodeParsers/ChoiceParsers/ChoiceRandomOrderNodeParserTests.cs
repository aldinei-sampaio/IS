using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceRandomOrderNodeParserTests
{
    private readonly ChoiceRandomOrderNodeParser sut;
    private readonly IElementParser elementParser;

    public ChoiceRandomOrderNodeParserTests()
    {
        elementParser = A.Fake<IElementParser>(i => i.Strict());
        sut = new(elementParser);
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("randomorder");
        sut.Settings.ShouldBeNormal();
    }

    [Fact]
    public async Task ShouldSetParentContextRandomOrderToTrue()
    {
        var reader = A.Dummy<XmlReader>();
        var context = A.Fake<IParsingContext>(i => i.Strict());
        var parentContext = new ChoiceParentParsingContext();

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .DoesNothing();

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.Choice.RandomOrder.Should().BeTrue();
    }
}
