using IS.Reading.Navigation;
using IS.Reading.Parsing.NodeParsers;

namespace IS.Reading.Parsing;

public class RootBlockParserTests
{
    private readonly IDocumentReader reader;
    private readonly IParsingContext context;
    private readonly IElementParser elementParser;
    private readonly IElementParserSettings elementParserSettings;
    private readonly IElementParserSettingsFactory elementParserSettingsFactory;
    private readonly RootBlockParser sut;

    public RootBlockParserTests()
    {
        reader = A.Dummy<IDocumentReader>();
        context = A.Fake<IParsingContext>(i => i.Strict());
        elementParser = A.Fake<IElementParser>(i => i.Strict());
        elementParserSettings = A.Fake<IElementParserSettings>(i => i.Strict());
        elementParserSettingsFactory = A.Fake<IElementParserSettingsFactory>(i => i.Strict());
        A.CallTo(() => elementParserSettingsFactory.NoBlock).Returns(elementParserSettings);

        sut = new(elementParser, elementParserSettingsFactory);
    }

    [Fact]
    public void Initialization()
    {
        sut.ElementParser.Should().BeSameAs(elementParser);
        sut.ElementParserSettingsFactory.Should().BeSameAs(elementParserSettingsFactory);
    }

    [Fact]
    public async Task SimpleParsing()
    {
        var parsed = A.Dummy<INode>();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, elementParserSettings))
            .Invokes(i => i.Arguments.Get<IParentParsingContext>(2).AddNode(parsed));

        var result = await sut.ParseAsync(reader, context);
        result.Should().ContainSingle().Which.Should().BeSameAs(parsed);
    }

    [Fact]
    public async Task Empty()
    {
        A.CallTo(() => context.LogError(reader, "Elemento filho era esperado.")).DoesNothing();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, elementParserSettings)).DoesNothing();

        var result = await sut.ParseAsync(reader, context);
        result.Should().BeEmpty();

        A.CallTo(() => context.LogError(reader, "Elemento filho era esperado.")).MustHaveHappenedOnceExactly();
    }
}
