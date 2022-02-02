using IS.Reading.Parsing.ArgumentParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceDefaultNodeParserTests
{
    private readonly ChoiceDefaultNodeParser sut;
    private readonly IElementParser elementParser;
    private readonly INameTextParser nameTextParser;

    public ChoiceDefaultNodeParserTests()
    {
        elementParser = A.Fake<IElementParser>(i => i.Strict());
        nameTextParser = A.Dummy<INameTextParser>();
        sut = new(elementParser, nameTextParser);
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("default");
        sut.Settings.ShouldBeNormal(nameTextParser);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("a")]
    [InlineData("u")]
    public async Task ShouldUpdateParentContextWithParsedText(string parsedValue)
    {
        var reader = A.Dummy<XmlReader>();
        var context = A.Fake<IParsingContext>(i => i.Strict());
        var parentContext = new ChoiceParentParsingContext();

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.GetArgument<IParentParsingContext>(2).ParsedText = parsedValue);

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.Choice.Default.Should().Be(parsedValue);
    }
}
