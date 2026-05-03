using IS.Reading.Choices;
using IS.Reading.Parsing.ArgumentParsers;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceTimeLimitNodeParserTests
{
    private readonly ChoiceTimeLimitNodeParser sut;
    private readonly IIntegerArgumentParser integerArgumentParser;

    private readonly IDocumentReader documentReader;
    private readonly IParsingContext parsingContext;
    private readonly BuilderParentParsingContext<IChoicePrototype> parentParsingContext;

    public ChoiceTimeLimitNodeParserTests()
    {
        integerArgumentParser = A.Fake<IIntegerArgumentParser>(i => i.Strict());
        sut = new(integerArgumentParser);

        documentReader = A.Fake<IDocumentReader>(i => i.Strict());
        parsingContext = Helper.FakeParsingContext();
        parentParsingContext = new();
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("timelimit");
        sut.IsArgumentRequired.Should().BeTrue();
        sut.IntegerArgumentParser.Should().BeSameAs(integerArgumentParser);
    }

    [Fact]
    public async Task ShouldLogArgumentParserError()
    {
        var errorMessage = "Erro proposital.";
        var argument = "1234";
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => integerArgumentParser.Parse(
            argument, 
            ChoiceTimeLimitNodeParser.MinValue, 
            ChoiceTimeLimitNodeParser.MaxValue
        )).Returns(Result.Fail<int>(errorMessage));
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.Builders.Should().BeEmpty();
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ParsingSuccess()
    {
        var argument = "1234";
        var parsed = 1234;

        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => integerArgumentParser.Parse(
            argument,
            ChoiceTimeLimitNodeParser.MinValue,
            ChoiceTimeLimitNodeParser.MaxValue
        )).Returns(Result.Ok(parsed));

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.Builders.Should().ContainSingle()
            .Which.Should().BeOfType<ChoiceTimeLimitSetter>()
            .Which.Value.Should().Be(TimeSpan.FromMilliseconds(parsed));
    }
}
