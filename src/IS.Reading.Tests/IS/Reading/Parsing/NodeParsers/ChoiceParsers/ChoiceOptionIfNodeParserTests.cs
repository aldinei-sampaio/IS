using IS.Reading.Choices;
using IS.Reading.Conditions;
using IS.Reading.Parsing.ConditionParsers;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class ChoiceOptionIfNodeParserTests
{
    private readonly IElementParser elementParser;
    private readonly IConditionParser conditionParser;
    private readonly IChoiceOptionTextNodeParser choiceOptionTextNodeParser;
    private readonly IChoiceOptionDisabledNodeParser choiceOptionDisabledNodeParser;
    private readonly IChoiceOptionIconNodeParser choiceOptionIconNodeParser;
    private readonly IChoiceOptionTipNodeParser choiceOptionTipNodeParser;
    private readonly ChoiceOptionIfNodeParser sut;


    private readonly IDocumentReader documentReader;
    private readonly IParsingContext parsingContext;
    private readonly ChoiceOptionParentParsingContext parentParsingContext;

    public ChoiceOptionIfNodeParserTests()
    {
        elementParser = A.Fake<IElementParser>(i => i.Strict(StrictFakeOptions.AllowToString));
        conditionParser = A.Fake<IConditionParser>(i => i.Strict(StrictFakeOptions.AllowToString));
        choiceOptionTextNodeParser = Helper.FakeParser<IChoiceOptionTextNodeParser>("a");
        choiceOptionDisabledNodeParser = Helper.FakeParser<IChoiceOptionDisabledNodeParser>("disabled");
        choiceOptionIconNodeParser = Helper.FakeParser<IChoiceOptionIconNodeParser>("icon");
        choiceOptionTipNodeParser = Helper.FakeParser<IChoiceOptionTipNodeParser>("tip");

        sut = new(
            elementParser, 
            conditionParser, 
            choiceOptionTextNodeParser, 
            choiceOptionDisabledNodeParser,
            choiceOptionIconNodeParser, 
            choiceOptionTipNodeParser
        );

        documentReader = A.Fake<IDocumentReader>(i => i.Strict());
        parsingContext = Helper.FakeParsingContext();
        parentParsingContext = new ChoiceOptionParentParsingContext();
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("if");
        sut.IsArgumentRequired.Should().BeTrue();
        sut.ElementParser.Should().BeSameAs(elementParser);
        sut.ConditionParser.Should().BeSameAs(conditionParser);
        sut.IfBlockSettings.Should().BeOfType<ElementParserSettings.IfBlock>();
        sut.IfBlockSettings.ChildParsers.Should().BeEquivalentTo(
            choiceOptionTextNodeParser,
            choiceOptionDisabledNodeParser,
            choiceOptionIconNodeParser,
            choiceOptionTipNodeParser,
            sut
        );
        sut.ElseBlockSettings.Should().BeOfType<ElementParserSettings.Block>();
        sut.ElseBlockSettings.ChildParsers.Should().BeEquivalentTo(
            choiceOptionTextNodeParser,
            choiceOptionDisabledNodeParser,
            choiceOptionIconNodeParser,
            choiceOptionTipNodeParser,
            sut
        );
    }

    [Fact]
    public async Task ShouldLogArgumentParserError()
    {
        var errorMessage = "Erro proposital.";
        var argument = "alpha = 1";
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => conditionParser.Parse(argument)).Returns(Result.Fail<ICondition>(errorMessage));
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.Builders.Should().BeEmpty();
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ShouldExitOnEventParserError()
    {
        var argument = "alpha = 1";
        var parsed = A.Dummy<ICondition>();
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => conditionParser.Parse(argument)).Returns(Result.Ok(parsed));
        A.CallTo(() => parsingContext.IsSuccess).Returns(false);
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.IfBlockSettings))
            .DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.Builders.Should().BeEmpty();
    }

    [Fact]
    public async Task NoBuildersParsed()
    {
        var argument = "alpha = 1";
        var condition = A.Dummy<ICondition>();

        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => conditionParser.Parse(argument)).Returns(Result.Ok(condition));
        A.CallTo(() => parsingContext.IsSuccess).Returns(false);
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.IfBlockSettings))
            .DoesNothing();
        A.CallTo(() => documentReader.Command).Returns("end");

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.Builders.Should().BeEmpty();
    }

    [Fact]
    public async Task ParsingSuccessWithoutElse()
    {
        var argument = "alpha = 1";
        var condition = A.Dummy<ICondition>();
        var builder = A.Dummy<IBuilder<IChoiceOptionPrototype>>();

        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => conditionParser.Parse(argument)).Returns(Result.Ok(condition));
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.IfBlockSettings))
            .Invokes(i => i.GetArgument<ChoiceOptionParentParsingContext>(2).Builders.Add(builder));
        A.CallTo(() => documentReader.Command).Returns("end");
        A.CallTo(() => documentReader.AtEnd).Returns(false);

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.Builders.Should().ContainSingle()
            .Which.Should().BeOfType<BuilderDecision<IChoiceOptionPrototype>>()
            .Which.ShouldSatisfy(i =>
            {
                i.Condition.Should().BeSameAs(condition);
                i.IfBlock.Should().BeEquivalentTo(builder);
                i.ElseBlock.Should().BeEmpty();
            });
    }

    [Fact]
    public async Task ParsingSuccessWithElse()
    {
        var argument = "alpha = 1";
        var condition = A.Dummy<ICondition>();
        var builder1 = A.Dummy<IBuilder<IChoiceOptionPrototype>>();
        var builder2 = A.Dummy<IBuilder<IChoiceOptionPrototype>>();

        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => conditionParser.Parse(argument)).Returns(Result.Ok(condition));
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.IfBlockSettings))
            .Invokes(i => i.GetArgument<ChoiceOptionParentParsingContext>(2).Builders.Add(builder1));
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.ElseBlockSettings))
            .Invokes(i => i.GetArgument<ChoiceOptionParentParsingContext>(2).Builders.Add(builder2));
        A.CallTo(() => documentReader.Command).Returns("else");
        A.CallTo(() => documentReader.AtEnd).Returns(false);

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.Builders.Should().ContainSingle()
            .Which.Should().BeOfType<BuilderDecision<IChoiceOptionPrototype>>()
            .Which.ShouldSatisfy(i =>
            {
                i.Condition.Should().BeSameAs(condition);
                i.IfBlock.Should().BeEquivalentTo(builder1);
                i.ElseBlock.Should().BeEquivalentTo(builder2);
            });
    }

    [Fact]
    public async Task ShouldExitOnElseParsingError()
    {
        var argument = "alpha = 1";
        var condition = A.Dummy<ICondition>();
        var builder1 = A.Dummy<IBuilder<IChoiceOptionPrototype>>();

        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => conditionParser.Parse(argument)).Returns(Result.Ok(condition));
        A.CallTo(() => parsingContext.IsSuccess).ReturnsNextFromSequence(true, false);
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.IfBlockSettings))
            .Invokes(i => i.GetArgument<ChoiceOptionParentParsingContext>(2).Builders.Add(builder1));
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.ElseBlockSettings))
            .DoesNothing();
        A.CallTo(() => documentReader.Command).Returns("else");
        A.CallTo(() => documentReader.AtEnd).Returns(false);

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.Builders.Should().BeEmpty();
    }
}
