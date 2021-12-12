using IS.Reading.Parsing.TextParsers;

namespace IS.Reading.Parsing.NodeParsers.PersonParsers;

public class PersonTextNodeParserTests
{
    private readonly IElementParser elementParser;
    private readonly INameTextParser nameTextParser;
    private readonly PersonTextNodeParser sut;

    public PersonTextNodeParserTests()
    {
        elementParser = A.Dummy<IElementParser>();
        nameTextParser = A.Dummy<INameTextParser>();
        sut = new(elementParser, nameTextParser);
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("person");
        sut.Settings.ShouldBeNormal(nameTextParser);
        sut.Should().BeAssignableTo<GenericTextNodeParserBase>();
    }
}
