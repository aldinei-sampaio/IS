using IS.Reading.Nodes;
using IS.Reading.Variables;

namespace IS.Reading.Parsing.NodeParsers;

public class SetNodeParserTests
{
    private readonly IDocumentReader documentReader;
    private readonly IParsingContext parsingContext;
    private readonly FakeParentParsingContext parentParsingContext;

    private readonly IVarSetParser varSetParser;
    private readonly SetNodeParser sut;

    public SetNodeParserTests()
    {
        varSetParser = A.Fake<IVarSetParser>(i => i.Strict());
        sut = new(varSetParser);

        documentReader = A.Fake<IDocumentReader>(i => i.Strict());
        parsingContext = Helper.FakeParsingContext();
        parentParsingContext = new FakeParentParsingContext();
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("set");
        sut.IsArgumentRequired.Should().BeTrue();
        sut.VarSetParser.Should().BeSameAs(varSetParser);
    }

    [Fact]
    public async Task ShouldLogNameParserError()
    {
        var errorMessage = "Erro proposital.";
        var argument = "a = 1";
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => varSetParser.Parse(argument)).Returns(Result.Fail<IVarSet>(errorMessage));
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldBeEmpty();
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ParsingSuccess()
    {
        var argument = "a = 1";
        var varSet = A.Dummy<IVarSet>();

        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => varSetParser.Parse(argument)).Returns(Result.Ok(varSet));

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldContainSingle<VarSetNode>(
            i => i.VarSet.Should().BeSameAs(varSet)
        );
    }
}
