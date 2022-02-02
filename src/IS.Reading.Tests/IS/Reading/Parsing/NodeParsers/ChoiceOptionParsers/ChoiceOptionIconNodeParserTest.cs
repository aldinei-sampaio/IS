using IS.Reading.Choices;
using IS.Reading.Parsing.ArgumentParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers.ChoiceOptionParsers;

public class ChoiceOptionIconNodeParserTest
{
    private readonly ChoiceOptionIconNodeParser sut;
    private readonly IElementParser elementParser;
    private readonly INameTextParser nameTextParser;

    public ChoiceOptionIconNodeParserTest()
    {
        elementParser = A.Fake<IElementParser>(i => i.Strict());
        nameTextParser = A.Dummy<INameTextParser>();
        sut = new(elementParser, nameTextParser);
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("icon");
        sut.Settings.ShouldBeNormal(nameTextParser);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("abraxas")]
    [InlineData("abracadabra")]
    public async Task ShouldUpdateParentContextIcon(string imageName)
    {
        var reader = A.Dummy<XmlReader>();
        var context = A.Fake<IParsingContext>(i => i.Strict());

        var optionNode = A.Fake<IChoiceOptionNodeSetter>(i => i.Strict());
        A.CallToSet(() => optionNode.ImageName).To(imageName).DoesNothing();

        var parentContext = A.Fake<IChoiceOptionParentParsingContext>(i => i.Strict());
        A.CallTo(() => parentContext.Option).Returns(optionNode);

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.GetArgument<IParentParsingContext>(2).ParsedText = imageName);

        await sut.ParseAsync(reader, context, parentContext);

        A.CallToSet(() => optionNode.ImageName).To(imageName).MustHaveHappenedOnceExactly();
    }    
}
