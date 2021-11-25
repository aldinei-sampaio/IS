using IS.Reading.Navigation;
using IS.Reading.Parsing.AttributeParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers;

public class BlockNodeParserTests
{
    private readonly XmlReader reader;
    private readonly IParsingContext context;
    private readonly IElementParser elementParser;
    private readonly IWhenAttributeParser whenAttributeParser;
    private readonly IWhileAttributeParser whileAttributeParser;
    private readonly IBackgroundNodeParser backgroundNodeParser;
    private readonly IPauseNodeParser pauseNodeParser;
    private readonly BlockNodeParser sut;

    public BlockNodeParserTests()
    {
        reader = A.Dummy<XmlReader>();
        context = A.Fake<IParsingContext>(i => i.Strict());
        elementParser = A.Fake<IElementParser>(i => i.Strict());

        whenAttributeParser = A.Dummy<IWhenAttributeParser>();
        A.CallTo(() => whenAttributeParser.AttributeName).Returns("when");
        
        whileAttributeParser = A.Dummy<IWhileAttributeParser>();
        A.CallTo(() => whileAttributeParser.AttributeName).Returns("while");
        
        backgroundNodeParser = A.Dummy<IBackgroundNodeParser>();
        A.CallTo(() => backgroundNodeParser.ElementName).Returns("background");
        
        pauseNodeParser = A.Dummy<IPauseNodeParser>();
        A.CallTo(() => pauseNodeParser.ElementName).Returns("pause");

        sut = new(
            elementParser, 
            whenAttributeParser, 
            whileAttributeParser, 
            backgroundNodeParser, 
            pauseNodeParser
        );
    }

    [Fact]
    public void Initialization()
    {
        sut.ElementName.Should().Be("do");
        sut.Settings.AttributeParsers["when"].Should().BeSameAs(whenAttributeParser);
        sut.Settings.AttributeParsers["while"].Should().BeSameAs(whileAttributeParser);
        sut.Settings.AttributeParsers.Should().HaveCount(2);
        sut.Settings.ChildParsers["background"].Should().BeSameAs(backgroundNodeParser);
        sut.Settings.ChildParsers["do"].Should().BeSameAs(sut);
        sut.Settings.ChildParsers["pause"].Should().BeSameAs(pauseNodeParser);
        sut.Settings.ChildParsers.Should().HaveCount(3);
        sut.Settings.TextParser.Should().BeNull();
    }

    [Fact]
    public async Task SimpleParsing()
    {
        var parsed = A.Dummy<IElementParsedData>();
        parsed.Block.ForwardQueue.Enqueue(A.Dummy<INode>());

        A.CallTo(() => elementParser.ParseAsync(reader, context, sut.Settings)).Returns(parsed);

        var result = await sut.ParseAsync(reader, context);

        result.Should().NotBeNull();
        result.When.Should().BeSameAs(parsed.When);
        result.While.Should().BeSameAs(parsed.While);
        result.ChildBlock.Should().BeSameAs(parsed.Block);
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
