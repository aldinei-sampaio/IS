using IS.Reading.Conditions;
using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.ConditionParsers;

namespace IS.Reading.Parsing.NodeParsers;

public class WhileNodeParserTests
{
    private readonly IDocumentReader documentReader;
    private readonly IParsingContext parsingContext;
    private readonly FakeParentParsingContext parentParsingContext = new();
    private readonly IElementParser elementParser;
    private readonly IElementParserSettings elementParserSettings;
    private readonly IElementParserSettingsFactory elementParserSettingsFactory;
    private readonly IConditionParser conditionParser;
    private readonly IBlockFactory blockFactory;
    private readonly WhileNodeParser sut;

    public WhileNodeParserTests()
    {
        elementParserSettings = A.Fake<IElementParserSettings>(i => i.Strict());
        elementParserSettingsFactory = A.Fake<IElementParserSettingsFactory>(i => i.Strict());
        A.CallTo(() => elementParserSettingsFactory.Block).Returns(elementParserSettings);
        elementParser = A.Fake<IElementParser>(i => i.Strict());
        conditionParser = A.Fake<IConditionParser>(i => i.Strict());
        
        documentReader = A.Fake<IDocumentReader>(i => i.Strict());
        parsingContext = A.Fake<IParsingContext>(i => i.Strict());
        blockFactory = A.Fake<IBlockFactory>(i => i.Strict());
        A.CallTo(() => blockFactory.Create(A<IReadOnlyList<INode>>.Ignored, A<ICondition>.Ignored)).ReturnsLazily(i =>
        {
            var nodes = i.GetArgument<IReadOnlyList<INode>>(0);
            var condition = i.GetArgument<ICondition>(1);
            var block = A.Dummy<IBlock>();
            A.CallTo(() => block.Nodes).Returns(nodes);
            A.CallTo(() => block.While).Returns(condition);
            return block;
        });
        A.CallTo(() => parsingContext.BlockFactory).Returns(blockFactory);

        sut = new(elementParser, conditionParser, elementParserSettingsFactory);
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("while");
        sut.IsArgumentRequired.Should().BeTrue();
        sut.ConditionParser.Should().BeSameAs(conditionParser);
        sut.ElementParser.Should().BeSameAs(elementParser);
        sut.ElementParserSettingsFactory.Should().BeSameAs(elementParserSettingsFactory);
    }

    [Fact]
    public async Task ShouldLogArgumentParsingError()
    {
        var errorMessage = "Erro proposital.";
        var argument = "Gibberish";
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => conditionParser.Parse(argument)).Returns(Result.Fail<ICondition>(errorMessage));
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldBeEmpty();
        A.CallTo(() => parsingContext.LogError(documentReader, errorMessage)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ShouldExitOnBlockParsingError()
    {
        var argument = "a = b";

        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => parsingContext.IsSuccess).Returns(false);

        var condition = A.Dummy<ICondition>();
        A.CallTo(() => conditionParser.Parse(argument)).Returns(Result.Ok(condition));

        var ifBlockNode = A.Dummy<INode>();
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, elementParserSettings))
            .DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldBeEmpty();
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, elementParserSettings))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task Success()
    {
        var argument = "b = c";

        A.CallTo(() => documentReader.AtEnd).Returns(false);
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);

        var condition = A.Dummy<ICondition>();
        A.CallTo(() => conditionParser.Parse(argument)).Returns(Result.Ok(condition));

        var blockNode = A.Dummy<INode>();
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, elementParserSettings))
            .Invokes(i => i.Arguments.Get<IParentParsingContext>(2).AddNode(blockNode));

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldContainSingle<BlockNode>(i =>
        {
            i.ChildBlock.While.Should().BeSameAs(condition);
            i.ChildBlock.ShouldContainOnly(blockNode);
        });
    }

}

