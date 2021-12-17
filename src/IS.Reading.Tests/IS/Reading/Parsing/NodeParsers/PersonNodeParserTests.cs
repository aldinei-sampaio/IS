using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.NodeParsers.PersonParsers;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers;

public class PersonNodeParserTests
{
    private readonly IElementParser elementParser;
    private readonly IPersonTextNodeParser personTextNodeParser;
    private readonly ISpeechNodeParser speechNodeParser;
    private readonly IThoughtNodeParser thoughtNodeParser;
    private readonly IMoodNodeParser moodNodeParser;
    private readonly IPauseNodeParser pauseNodeParser;
    private readonly PersonNodeParser sut;
    
    public PersonNodeParserTests()
    {
        elementParser = A.Fake<IElementParser>(i => i.Strict());
        personTextNodeParser = Helper.FakeParser<IPersonTextNodeParser>("person");
        speechNodeParser = Helper.FakeParser<ISpeechNodeParser>("speech");
        thoughtNodeParser = Helper.FakeParser<IThoughtNodeParser>("thought");
        moodNodeParser = Helper.FakeParser<IMoodNodeParser>("mood");
        pauseNodeParser = Helper.FakeParser<IPauseNodeParser>("pause");
        sut = new(elementParser, personTextNodeParser, speechNodeParser, thoughtNodeParser, moodNodeParser, pauseNodeParser);
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("person");
        sut.Settings.ShouldBeAggregatedNonRepeat(personTextNodeParser);
        sut.AggregationSettings.ShouldBeAggregated(speechNodeParser, thoughtNodeParser, moodNodeParser, pauseNodeParser);
        sut.ResetMoodNode.Should().BeEquivalentTo(new { MoodType = (MoodType?)null });
    }

    [Fact]
    public async Task ParseAsyncShouldReturnAPersonNode()
    {
        var reader = A.Dummy<XmlReader>();
        var context = A.Dummy<IParsingContext>();
        var parentContext = new FakeParentParsingContext();

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.Arguments.Get<IParentParsingContext>(2).ParsedText = "lorenipsum");

        var dummyNode = A.Dummy<INode>();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.AggregationSettings))
            .Invokes(i => i.Arguments.Get<IParentParsingContext>(2).AddNode(dummyNode));

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.ShouldContainSingle<PersonNode>(i =>
            {
                i.PersonName.Should().Be("lorenipsum");
                i.ChildBlock.ShouldContain(dummyNode, sut.ResetMoodNode);
            }
        );

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.AggregationSettings))
            .MustHaveHappenedOnceExactly();
    }

    // TODO: Refatorar nomes de unittest uma vez que ParseAsync não tem mais valor de retorno
    [Fact]
    public async Task ParseAsyncShouldReturnNullIfParsedTextIsNull()
    {
        var reader = A.Dummy<XmlReader>();
        var context = A.Dummy<IParsingContext>();
        var parentContext = new FakeParentParsingContext();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings)).DoesNothing();

        await sut.ParseAsync(reader, context, parentContext);
        parentContext.ShouldBeEmpty();
    }

    [Fact]
    public async Task ParseAsyncShouldLogErrorIfParsedTextIsEmpty()
    {
        var errorMessage = "Era esperado o nome do personagem.";
        var reader = A.Dummy<XmlReader>();
        var context = A.Fake<IParsingContext>(i => i.Strict());
        var parentContext = new FakeParentParsingContext();

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.Arguments.Get<IParentParsingContext>(2).ParsedText = string.Empty);

        A.CallTo(() => context.LogError(reader, errorMessage)).DoesNothing();

        await sut.ParseAsync(reader, context, parentContext);
        parentContext.ShouldBeEmpty();

        A.CallTo(() => context.LogError(reader, errorMessage)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ShouldNotCreateNodeWhenThereAreNoMoreElements()
    {
        var reader = A.Dummy<XmlReader>();
        var context = A.Dummy<IParsingContext>();
        var parentContext = new FakeParentParsingContext();

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.Arguments.Get<IParentParsingContext>(2).ParsedText = "lorenipsum");

        // Se depois do elementParser.ParseAsync o ReadState for EndOfFile significa que não há mais elementos
        // dentro do elemento atual
        A.CallTo(() => reader.ReadState).Returns(ReadState.EndOfFile);

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.ShouldBeEmpty();

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ShouldNotCreateNodeWhenThereAreNoAggregatableElements()
    {
        var reader = A.Dummy<XmlReader>();
        var context = A.Dummy<IParsingContext>();
        var parentContext = new FakeParentParsingContext();

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .Invokes(i => i.Arguments.Get<IParentParsingContext>(2).ParsedText = "lorenipsum");

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.AggregationSettings))
            .DoesNothing();

        await sut.ParseAsync(reader, context, parentContext);

        parentContext.ShouldBeEmpty();

        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.Settings))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => elementParser.ParseAsync(reader, context, A<IParentParsingContext>.Ignored, sut.AggregationSettings))
            .MustHaveHappenedOnceExactly();
    }
}
