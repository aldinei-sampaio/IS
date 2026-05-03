using IS.Reading.Choices;
using IS.Reading.Parsing.ArgumentParsers;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceOptionIconNodeParserTests
{
    private readonly ChoiceOptionIconNodeParser sut;
    private readonly INameArgumentParser nameArgumentParser;

    private readonly IDocumentReader documentReader;
    private readonly IParsingContext parsingContext;
    private readonly BuilderParentParsingContext<IChoiceOptionPrototype> parentParsingContext;

    public ChoiceOptionIconNodeParserTests()
    {
        nameArgumentParser = A.Fake<INameArgumentParser>(i => i.Strict());
        sut = new(nameArgumentParser);

        documentReader = A.Fake<IDocumentReader>(i => i.Strict());
        parsingContext = Helper.FakeParsingContext();
        parentParsingContext = new();
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("icon");
        sut.IsArgumentRequired.Should().BeTrue();
        sut.NameArgumentParser.Should().BeSameAs(nameArgumentParser);
    }

    [Fact]
    public async Task ShouldLogArgumentParserError()
    {
        var errorMessage = "Erro proposital.";
        var argument = "c";
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => nameArgumentParser.Parse(argument)).Returns(Result.Fail<string>(errorMessage));
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.Builders.Should().BeEmpty();
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ParsingSuccess()
    {
        var argument = "C";
        var parsed = "c";

        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => nameArgumentParser.Parse(argument)).Returns(Result.Ok(parsed));

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.Builders.Should().ContainSingle()
            .Which.Should().BeOfType<ChoiceOptionImageNameSetter>()
            .Which.Value.Should().Be(parsed);
    }
}
