using IS.Reading.Nodes;
using IS.Reading.Parsing.ArgumentParsers;
using IS.Reading.Parsing.NodeParsers.InputParsers;
using IS.Reading.Variables;

namespace IS.Reading.Parsing.NodeParsers;

public class InputNodeParserTests
{
    private readonly IDocumentReader documentReader;
    private readonly IParsingContext parsingContext;
    private readonly FakeParentParsingContext parentParsingContext;

    private readonly IElementParser elementParser;
    private readonly INameArgumentParser nameArgumentParser;
    private readonly IInputTitleNodeParser inputTitleNodeParser;
    private readonly IInputTextNodeParser inputTextNodeParser;
    private readonly IInputLenNodeParser inputLenNodeParser;
    private readonly IInputConfNodeParser inputConfNodeParser;
    private readonly InputNodeParser sut;

    public InputNodeParserTests()
    {
        documentReader = A.Fake<IDocumentReader>(i => i.Strict());
        parsingContext = A.Fake<IParsingContext>(i => i.Strict());
        parentParsingContext = new();

        elementParser = A.Fake<IElementParser>(i => i.Strict());
        nameArgumentParser = A.Fake<INameArgumentParser>(i => i.Strict());
        inputTitleNodeParser = Helper.FakeParser<IInputTitleNodeParser>("title");
        inputTextNodeParser = Helper.FakeParser<IInputTextNodeParser>("text");
        inputLenNodeParser = Helper.FakeParser<IInputLenNodeParser>("len");
        inputConfNodeParser = Helper.FakeParser<IInputConfNodeParser>("conf");

        sut = new(
            elementParser, 
            nameArgumentParser, 
            inputTitleNodeParser, 
            inputTextNodeParser, 
            inputLenNodeParser, 
            inputConfNodeParser
        );
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("input");
        sut.IsArgumentRequired.Should().BeTrue();
        sut.ElementParser.Should().BeSameAs(elementParser);
        sut.NameArgumentParser.Should().BeSameAs(nameArgumentParser);
        sut.Settings.Should().BeOfType<ElementParserSettings.AggregatedNonRepeat>()
            .Which.ChildParsers.Should().BeEquivalentTo(
                inputTitleNodeParser,
                inputTextNodeParser,
                inputLenNodeParser,
                inputConfNodeParser
            );
    }

    [Fact]
    public async Task ShouldLogArgumentParserError()
    {
        var errorMessage = "Erro proposital.";
        var argument = "Gibberish";
        A.CallTo(() => nameArgumentParser.Parse(argument)).Returns(Result.Fail<string>(errorMessage));
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ShouldExitOnChildrenParsingError()
    {
        var argument = "Abobrinha";
        A.CallTo(() => nameArgumentParser.Parse(argument)).Returns(Result.Ok("abobrinha"));
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => elementParser.ParseAsync(
            documentReader, 
            parsingContext, 
            A<IParentParsingContext>.Ignored, 
            sut.Settings
        )).DoesNothing();
        A.CallTo(() => parsingContext.IsSuccess).Returns(false);

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldBeEmpty();
    }

    [Fact]
    public async Task WithoutChildren()
    {
        var argument = "Gororoba";
        var key = "gororoba";
        A.CallTo(() => nameArgumentParser.Parse(argument)).Returns(Result.Ok(key));
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => elementParser.ParseAsync(
            documentReader,
            parsingContext,
            A<IParentParsingContext>.Ignored,
            sut.Settings
        )).DoesNothing();
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldContainSingle<InputNode>(i =>
        {
            i.InputBuilder.Key.Should().Be(key);
            i.InputBuilder.TitleSource.Should().BeNull();
            i.InputBuilder.TextSource.Should().BeNull();
            i.InputBuilder.MaxLength.Should().Be(InputBuilder.MaxLenghtDefaultValue);
            i.InputBuilder.ConfirmationSource.Should().BeNull();
        });
    }

    [Fact]
    public async Task WithChildren()
    {
        var argument = "Gororoba";
        var key = "gororoba";
        A.CallTo(() => nameArgumentParser.Parse(argument)).Returns(Result.Ok(key));
        A.CallTo(() => documentReader.Argument).Returns(argument);

        var titleSource = A.Fake<ITextSource>(i => i.Strict());
        var textSource = A.Fake<ITextSource>(i => i.Strict());
        var maxLength = 21;
        var confirmationSource = A.Fake<ITextSource>(i => i.Strict());
        A.CallTo(() => elementParser.ParseAsync(
            documentReader,
            parsingContext,
            A<IParentParsingContext>.Ignored,
            sut.Settings
        )).Invokes(i =>
        {
            var ctx = i.GetArgument<InputParentParsingContext>(2);
            ctx.InputBuilder.TitleSource = titleSource;
            ctx.InputBuilder.TextSource = textSource;
            ctx.InputBuilder.MaxLength = maxLength;
            ctx.InputBuilder.ConfirmationSource = confirmationSource;
        });

        A.CallTo(() => parsingContext.IsSuccess).Returns(true);

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldContainSingle<InputNode>(i =>
        {
            i.InputBuilder.Key.Should().Be(key);
            i.InputBuilder.TitleSource.Should().BeSameAs(titleSource);
            i.InputBuilder.TextSource.Should().BeSameAs(textSource);
            i.InputBuilder.MaxLength.Should().Be(maxLength);
            i.InputBuilder.ConfirmationSource.Should().BeSameAs(confirmationSource);
        });
    }
}
