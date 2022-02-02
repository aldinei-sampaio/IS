using IS.Reading.Nodes;
using IS.Reading.Parsing.ArgumentParsers;
using IS.Reading.Variables;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers;

public class UnsetNodeParserTests
{
    private readonly XmlReader reader;
    private readonly IParsingContext context;
    private readonly FakeParentParsingContext parentContext;

    private readonly IElementParser elementParser;
    private readonly INameTextParser nameTextParser;
    private readonly UnsetNodeParser sut;

    public UnsetNodeParserTests()
    {
        reader = A.Dummy<XmlReader>();
        context = A.Fake<IParsingContext>(i => i.Strict());
        parentContext = new();

        elementParser = A.Fake<IElementParser>(i => i.Strict());
        nameTextParser = A.Fake<INameTextParser>(i => i.Strict());
        sut = new(elementParser, nameTextParser);
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("unset");
        sut.Settings.ShouldBeNormal(nameTextParser);
    }

    [Fact]
    public async Task ShouldDoNothingIfTextParserReturnsNull()
    {
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .DoesNothing();

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.ShouldBeEmpty();
    }

    [Fact]
    public async Task ShouldReturnVarSetNodeWhenParsingIsSuccessful()
    {
        const string parsedText = "alpha";

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.GetArgument<IParentParsingContext>(2).ParsedText = parsedText);

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.ShouldContainSingle<VarSetNode>(
            i => i.VarSet.Should().BeOfType<VarSet>().Which.Should().BeEquivalentTo(new
            {
                Name = parsedText,
                Value = (object)null
            })
        );
    }
}
