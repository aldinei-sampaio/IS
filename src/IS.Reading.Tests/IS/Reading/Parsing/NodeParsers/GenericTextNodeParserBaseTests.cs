using System.Xml;

namespace IS.Reading.Parsing.NodeParsers;

public class GenericTextNodeParserBaseTests
{
    private class TestClass : GenericTextNodeParserBase
    {
        public TestClass(IElementParser elementParser, ITextParser textParser) : base(elementParser, textParser)
        {
        }

        public override string Name => "teste";
    }

    private readonly TestClass sut;
    private readonly IElementParser elementParser;
    private readonly ITextParser textParser;

    public GenericTextNodeParserBaseTests()
    {
        elementParser = A.Fake<IElementParser>(i => i.Strict());
        textParser = A.Dummy<ITextParser>();
        sut = new(elementParser, textParser);
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("teste");
        sut.Settings.ShouldBeNormal(textParser);
    }

    [Fact]
    public async Task ParseAsyncShouldDelegateAllToElementParser()
    {
        var context = A.Fake<IParsingContext>(i => i.Strict());
        var parentContext = new FakeParentParsingContext();
        var reader = A.Dummy<XmlReader>();

        A.CallTo(() => elementParser.ParseAsync(reader, context, parentContext, sut.Settings)).DoesNothing();

        await sut.ParseAsync(reader, context, parentContext);

        A.CallTo(() => elementParser.ParseAsync(reader, context, parentContext, sut.Settings)).MustHaveHappenedOnceExactly();
    }
}
