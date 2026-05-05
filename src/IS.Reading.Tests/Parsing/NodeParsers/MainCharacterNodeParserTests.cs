using IS.Reading.Nodes;
using IS.Reading.Parsing.ArgumentParsers;

namespace IS.Reading.Parsing.NodeParsers;

public class MainCharacterNodeParserTests
{
    private readonly IDocumentReader documentReader;
    private readonly IParsingContext parsingContext;
    private readonly FakeParentParsingContext parentParsingContext;

    private readonly INameArgumentParser nameArgumentParser;
    private readonly MainCharacterNodeParser sut;

    public MainCharacterNodeParserTests()
    {
        nameArgumentParser = A.Fake<INameArgumentParser>(i => i.Strict());
        sut = new(nameArgumentParser);

        documentReader = A.Fake<IDocumentReader>(i => i.Strict());
        parsingContext = Helper.FakeParsingContext();
        parentParsingContext = new FakeParentParsingContext();
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("mc");
        sut.IsArgumentRequired.Should().BeTrue();
        sut.NameArgumentParser.Should().BeSameAs(nameArgumentParser);
        sut.DismissNode.Should().BeOfType<MainCharacterNode>().Which.MainCharacterName.Should().BeNull();
    }

    [Fact]
    public async Task ShouldLogNameParserError()
    {
        var errorMessage = "Erro proposital.";
        var argument = "johan";
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => nameArgumentParser.Parse(argument)).Returns(Result.Fail<string>(errorMessage));
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldBeEmpty();
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ParsingSuccess()
    {
        var argument = "Johan";
        var parsed = "johan";
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => nameArgumentParser.Parse(argument)).Returns(Result.Ok(parsed));
        A.CallTo(() => parsingContext.RegisterDismissNode(sut.DismissNode)).DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldContainSingle<MainCharacterNode>(
            i => i.MainCharacterName.Should().Be(parsed)
        );

        A.CallTo(() => parsingContext.RegisterDismissNode(sut.DismissNode)).MustHaveHappenedOnceExactly();
    }
}
