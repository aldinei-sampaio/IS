using IS.Reading.Choices;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceOptionDisabledNodeParserTests
{
    private readonly ChoiceOptionDisabledNodeParser sut;

    private readonly IDocumentReader documentReader;
    private readonly IParsingContext parsingContext;
    private readonly ChoiceOptionParentParsingContext parentParsingContext;

    public ChoiceOptionDisabledNodeParserTests()
    {
        sut = new();

        documentReader = A.Fake<IDocumentReader>(i => i.Strict());
        parsingContext = Helper.FakeParsingContext();
        parentParsingContext = new ChoiceOptionParentParsingContext();
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("disabled");
        sut.IsArgumentRequired.Should().BeFalse();        
    }

    [Fact]
    public async void ShouldLogErrorWhenThereIsArgument()
    {
        var errorMessage = "O comando 'disabled' não suporta argumento.";

        A.CallTo(() => documentReader.Argument).Returns("qualquer coisa");
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.Builders.Should().BeEmpty();
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async void SuccessParsing()
    {
        A.CallTo(() => documentReader.Argument).Returns(string.Empty);

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.Builders.Should().ContainSingle()
            .Which.Should().BeOfType<ChoiceOptionIsEnabledSetter>()
            .Which.Value.Should().BeFalse();
    }
}
