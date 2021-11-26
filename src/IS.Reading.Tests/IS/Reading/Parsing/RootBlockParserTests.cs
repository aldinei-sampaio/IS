using IS.Reading.Navigation;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers;

public class RootBlockParserTests
{
    private readonly XmlReader reader;
    private readonly IParsingContext context;
    private readonly IElementParser elementParser;
    private readonly IBackgroundNodeParser backgroundNodeParser;
    private readonly IBlockNodeParser blockNodeParser;
    private readonly IPauseNodeParser pauseNodeParser;
    private readonly RootBlockParser sut;

    public RootBlockParserTests()
    {
        reader = A.Dummy<XmlReader>();
        context = A.Fake<IParsingContext>(i => i.Strict());
        elementParser = A.Fake<IElementParser>(i => i.Strict());

        backgroundNodeParser = Helper.FakeParser<IBackgroundNodeParser>("background");
        blockNodeParser = Helper.FakeParser<IBlockNodeParser>("do");
        pauseNodeParser = Helper.FakeParser<IPauseNodeParser>("pause");

        sut = new(
            elementParser, 
            backgroundNodeParser, 
            blockNodeParser,
            pauseNodeParser
        );
    }

    [Fact]
    public void Initialization()
    {
        sut.Settings.AttributeParsers.Count.Should().Be(0);
        sut.Settings.ChildParsers["background"].Should().BeSameAs(backgroundNodeParser);
        sut.Settings.ChildParsers["do"].Should().BeSameAs(blockNodeParser);
        sut.Settings.ChildParsers["pause"].Should().BeSameAs(pauseNodeParser);
        sut.Settings.ChildParsers.Count.Should().Be(3);
        sut.Settings.TextParser.Should().BeNull();
    }

    [Fact]
    public async Task SimpleParsing()
    {
        var parsed = A.Dummy<IElementParsedData>();
        parsed.Block.ForwardQueue.Enqueue(A.Dummy<INode>());

        A.CallTo(() => elementParser.ParseAsync(reader, context, sut.Settings)).Returns(parsed);

        var result = await sut.ParseAsync(reader, context);
        result.Should().BeSameAs(parsed.Block);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Empty(bool nullParsedBlock)
    {
        A.CallTo(() => context.LogError(reader, "Elemento filho era esperado.")).DoesNothing();

        var parsed = A.Dummy<IElementParsedData>();
        if (nullParsedBlock)
            parsed.Block = null;

        A.CallTo(() => elementParser.ParseAsync(reader, context, sut.Settings)).Returns(parsed);

        var result = await sut.ParseAsync(reader, context);
        result.Should().BeNull();

        A.CallTo(() => context.LogError(reader, "Elemento filho era esperado.")).MustHaveHappenedOnceExactly();
    }
}
