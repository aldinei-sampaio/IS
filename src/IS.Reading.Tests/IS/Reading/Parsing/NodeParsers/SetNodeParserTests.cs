using IS.Reading.Nodes;
using IS.Reading.Parsing.TextParsers;
using IS.Reading.Variables;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers;

public class SetNodeParserTests
{
    private readonly XmlReader reader;
    private readonly IParsingContext context;
    private readonly FakeParentParsingContext parentContext;

    private readonly IVarSetParser varSetParser;
    private readonly IElementParser elementParser;
    private readonly IVarSetTextParser varSetTextParser;
    private readonly SetNodeParser sut;

    public SetNodeParserTests()
    {
        reader = A.Dummy<XmlReader>();
        context = A.Fake<IParsingContext>(i => i.Strict());
        parentContext = new();

        varSetParser = A.Fake<IVarSetParser>(i => i.Strict());
        elementParser = A.Fake<IElementParser>(i => i.Strict());
        varSetTextParser = A.Fake<IVarSetTextParser>(i => i.Strict());
        sut = new(varSetParser, elementParser, varSetTextParser);
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("set");
        sut.Settings.ShouldBeNormal(varSetTextParser);
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
    public async Task ShouldLogErrorIfParserReturnsNull()
    {
        const string message = "Expressão de atribuição de variável inválida.";
        const string parsedText = "alpha=1";

        A.CallTo(() => context.LogError(reader, message)).DoesNothing();

        A.CallTo(() => varSetParser.Parse(parsedText)).Returns(null);

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.GetArgument<IParentParsingContext>(2).ParsedText = parsedText);

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.ShouldBeEmpty();

        A.CallTo(() => varSetParser.Parse(parsedText)).MustHaveHappenedOnceExactly();
        A.CallTo(() => context.LogError(reader, message)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ShouldReturnVarSetNodeWhenParsingIsSuccessful()
    {
        const string parsedText = "alpha=1";

        var varSet = A.Dummy<IVarSet>();

        A.CallTo(() => varSetParser.Parse(parsedText)).Returns(varSet);

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.GetArgument<IParentParsingContext>(2).ParsedText = parsedText);

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.ShouldContainSingle<VarSetNode>(i => i.VarSet.Should().BeSameAs(varSet));
    }
}
