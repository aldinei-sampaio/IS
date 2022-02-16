using IS.Reading.Conditions;
using IS.Reading.Nodes;

namespace IS.Reading.Parsing.NodeParsers.BackgroundParsers;

public class BackgroundScrollNodeParserTests
{
    private readonly IDocumentReader documentReader;
    private readonly IParsingContext parsingContext;
    private readonly FakeParentParsingContext parentParsingContext;

    private readonly BackgroundScrollNodeParser sut;

    public BackgroundScrollNodeParserTests()
    {
        sut = new();

        documentReader = A.Fake<IDocumentReader>(i => i.Strict());
        parsingContext = Helper.FakeParsingContext();
        parentParsingContext = new FakeParentParsingContext();
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("scroll");
        sut.IsArgumentRequired.Should().BeFalse();
    }

    [Fact]
    public async Task ShouldNotAcceptArgument()
    {
        var errorMessage = "O comando 'scroll' não suporta argumento.";
        var argument = "gibberish";
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldBeEmpty();
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Success()
    {
        A.CallTo(() => documentReader.Argument).Returns(string.Empty);

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldContainSingle<ScrollNode>();
    }
}
