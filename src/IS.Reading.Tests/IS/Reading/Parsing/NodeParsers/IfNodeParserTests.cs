using IS.Reading.Conditions;
using IS.Reading.Navigation;
using IS.Reading.Nodes;
using IS.Reading.Parsing.ConditionParsers;

namespace IS.Reading.Parsing.NodeParsers;

public class IfNodeParserTests
{
    private readonly IDocumentReader documentReader;
    private readonly IParsingContext parsingContext;
    private readonly FakeParentParsingContext parentParsingContext = new();
    private readonly IElementParser elementParser;
    private readonly IElementParserSettings ifSettings;
    private readonly IElementParserSettings elseSettings;
    private readonly IElementParserSettingsFactory elementParserSettingsFactory;
    private readonly IConditionParser conditionParser;
    private readonly IBlockFactory blockFactory;
    private readonly IfNodeParser sut;

    public IfNodeParserTests()
    {
        ifSettings = A.Fake<IElementParserSettings>(i => i.Strict(StrictFakeOptions.AllowObjectMethods));
        elseSettings = A.Fake<IElementParserSettings>(i => i.Strict(StrictFakeOptions.AllowObjectMethods));
        elementParserSettingsFactory = A.Fake<IElementParserSettingsFactory>(i => i.Strict());
        A.CallTo(() => elementParserSettingsFactory.IfBlock).Returns(ifSettings);
        A.CallTo(() => elementParserSettingsFactory.Block).Returns(elseSettings);
        elementParser = A.Fake<IElementParser>(i => i.Strict());
        conditionParser = A.Fake<IConditionParser>(i => i.Strict());

        documentReader = A.Fake<IDocumentReader>(i => i.Strict());
        parsingContext = A.Fake<IParsingContext>(i => i.Strict());
        blockFactory = A.Fake<IBlockFactory>(i => i.Strict());
        A.CallTo(() => blockFactory.Create(A<IReadOnlyList<INode>>.Ignored)).ReturnsLazily(i =>
        {
            var nodes = i.GetArgument<IReadOnlyList<INode>>(0);
            var block = A.Dummy<IBlock>();
            A.CallTo(() => block.Nodes).Returns(nodes);
            A.CallTo(() => block.While).Returns(null);
            return block;
        });
        A.CallTo(() => parsingContext.BlockFactory).Returns(blockFactory);

        sut = new(elementParser, conditionParser, elementParserSettingsFactory);
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("if");
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
    public async Task ShouldExitOnIfParsingError()
    {
        var argument = "a = b";

        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => parsingContext.IsSuccess).Returns(false);

        var condition = A.Dummy<ICondition>();
        A.CallTo(() => conditionParser.Parse(argument)).Returns(Result.Ok(condition));

        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, ifSettings))
            .DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldBeEmpty();
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, ifSettings))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ShouldExitOnElseIfParsingError()
    {
        var argument = "c = d";

        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => parsingContext.IsSuccess).ReturnsNextFromSequence(true, false);
        A.CallTo(() => documentReader.Command).Returns("elseif");
        A.CallTo(() => documentReader.AtEnd).Returns(false);

        var condition = A.Dummy<ICondition>();
        A.CallTo(() => conditionParser.Parse(argument)).Returns(Result.Ok(condition));

        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, ifSettings))
            .DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldBeEmpty();
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, ifSettings))
            .MustHaveHappenedTwiceExactly();
    }

    [Fact]
    public async Task ShouldExitOnElseParsingError()
    {
        var argument = "e = f";

        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => parsingContext.IsSuccess).ReturnsNextFromSequence(true, false);
        A.CallTo(() => documentReader.Command).Returns("else");
        A.CallTo(() => documentReader.AtEnd).Returns(false);

        var condition = A.Dummy<ICondition>();
        A.CallTo(() => conditionParser.Parse(argument)).Returns(Result.Ok(condition));

        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, ifSettings))
            .DoesNothing();
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, elseSettings))
            .DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldBeEmpty();
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, ifSettings))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, elseSettings))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task IfWithElse()
    {
        var argument = "m = n";

        A.CallTo(() => documentReader.AtEnd).Returns(false);
        A.CallTo(() => documentReader.Command).Returns("else");
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);

        var condition = A.Dummy<ICondition>();
        A.CallTo(() => conditionParser.Parse(argument)).Returns(Result.Ok(condition));

        var ifBlockNode = A.Dummy<INode>();
        var elseBlockNode = A.Dummy<INode>();
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, ifSettings))
            .Invokes(i => i.GetParentContext().AddNode(ifBlockNode));
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, elseSettings))
            .Invokes(i => i.GetParentContext().AddNode(elseBlockNode));

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldContainSingle<IfNode>(i =>
        {
            i.DecisionBlocks.Should().ContainSingle()
                .Which.Should().BeOfType<DecisionBlock>()
                .Which.ShouldSatisfy(i =>
                {
                    i.Condition.Should().BeSameAs(condition);
                    i.Block.ShouldContainOnly(ifBlockNode);
                });
            i.ElseBlock.ShouldContainOnly(elseBlockNode);
            i.ElseBlock.While.Should().BeNull();
            i.ChildBlock.Should().BeNull();
        });
    }

    [Fact]
    public async Task IfWithElseIf()
    {
        var argument = "m = n";

        A.CallTo(() => documentReader.AtEnd).Returns(false);
        A.CallTo(() => documentReader.Command).ReturnsNextFromSequence("elseif", "end");
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);

        var conditions = new[] { A.Dummy<ICondition>(), A.Dummy<ICondition>() };
        var nextCondition = 0;

        A.CallTo(() => conditionParser.Parse(argument)).ReturnsLazily(i => Result.Ok(conditions[nextCondition++]));

        var ifBlockNodes = new[] { A.Dummy<INode>(), A.Dummy<INode>() };
        var nextIfBlockNode = 0;

        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, ifSettings))
            .Invokes(i => i.GetParentContext().AddNode(ifBlockNodes[nextIfBlockNode++]));

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldContainSingle<IfNode>(i =>
        {
            i.DecisionBlocks.Should().Contain(
                i => i.Should().BeOfType<DecisionBlock>()
                    .Which.ShouldSatisfy(i =>
                    {
                        i.Condition.Should().BeSameAs(conditions[0]);
                        i.Block.ShouldContainOnly(ifBlockNodes[0]);
                    }),
                i => i.Should().BeOfType<DecisionBlock>()
                    .Which.ShouldSatisfy(i =>
                    {
                        i.Condition.Should().BeSameAs(conditions[1]);
                        i.Block.ShouldContainOnly(ifBlockNodes[1]);
                    })
            );
            i.ElseBlock.ShouldBeEmpty();
            i.ChildBlock.Should().BeNull();
        });
    }

    [Fact]
    public async Task IfWithElseIfAndElse()
    {
        var argument = "m = n";

        A.CallTo(() => documentReader.AtEnd).Returns(false);
        A.CallTo(() => documentReader.Command).ReturnsNextFromSequence("elseif", "else", "end");
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);

        var conditions = new[] { A.Dummy<ICondition>(), A.Dummy<ICondition>() };
        var nextCondition = 0;

        A.CallTo(() => conditionParser.Parse(argument)).ReturnsLazily(i => Result.Ok(conditions[nextCondition++]));

        var ifBlockNodes = new[] { A.Dummy<INode>(), A.Dummy<INode>() };
        var elseNode = A.Dummy<INode>();
        var nextIfBlockNode = 0;

        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, ifSettings))
            .Invokes(i => i.GetParentContext().AddNode(ifBlockNodes[nextIfBlockNode++]));
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, elseSettings))
            .Invokes(i => i.GetParentContext().AddNode(elseNode));

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldContainSingle<IfNode>(i =>
        {
            i.DecisionBlocks.Should().Contain(
                i => i.Should().BeOfType<DecisionBlock>()
                    .Which.ShouldSatisfy(i =>
                    {
                        i.Condition.Should().BeSameAs(conditions[0]);
                        i.Block.ShouldContainOnly(ifBlockNodes[0]);
                    }),
                i => i.Should().BeOfType<DecisionBlock>()
                    .Which.ShouldSatisfy(i =>
                    {
                        i.Condition.Should().BeSameAs(conditions[1]);
                        i.Block.ShouldContainOnly(ifBlockNodes[1]);
                    })
            );
            i.ElseBlock.ShouldContainOnly(elseNode);
            i.ChildBlock.Should().BeNull();
        });
    }

    [Fact]
    public async Task ShouldNotCreateIfNodeWhenThereIsNoNodes()
    {
        var argument = "m = n";

        A.CallTo(() => documentReader.AtEnd).Returns(false);
        A.CallTo(() => documentReader.Command).ReturnsNextFromSequence("elseif", "else", "end");
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);

        var conditions = new[] { A.Dummy<ICondition>(), A.Dummy<ICondition>() };
        var nextCondition = 0;

        A.CallTo(() => conditionParser.Parse(argument)).ReturnsLazily(i => Result.Ok(conditions[nextCondition++]));

        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, ifSettings))
            .DoesNothing();
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, elseSettings))
            .DoesNothing();

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldBeEmpty();
    }

    [Fact]
    public async Task IfWithoutElse()
    {
        var argument = "r = s";

        A.CallTo(() => documentReader.AtEnd).Returns(false);
        A.CallTo(() => documentReader.Command).Returns("end");
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);

        var condition = A.Dummy<ICondition>();
        A.CallTo(() => conditionParser.Parse(argument)).Returns(Result.Ok(condition));

        var ifBlockNode = A.Dummy<INode>();
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, ifSettings))
            .Invokes(i => i.GetParentContext().AddNode(ifBlockNode));

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldContainSingle<IfNode>(i =>
        {
            i.DecisionBlocks.Should().ContainSingle()
                .Which.Should().BeOfType<DecisionBlock>()
                .Which.ShouldSatisfy(i =>
                {
                    i.Condition.Should().BeSameAs(condition);
                    i.Block.ShouldContainOnly(ifBlockNode);
                });
            i.ElseBlock.ShouldBeEmpty();
            i.ChildBlock.Should().BeNull();
        });
    }

    [Fact]
    public async Task IfWithoutElseBecauseEndWasReached()
    {
        var argument = "u = v";

        A.CallTo(() => documentReader.AtEnd).Returns(true);
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);

        var condition = A.Dummy<ICondition>();
        A.CallTo(() => conditionParser.Parse(argument)).Returns(Result.Ok(condition));

        var ifBlockNode = A.Dummy<INode>();
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, ifSettings))
            .Invokes(i => i.Arguments.Get<IParentParsingContext>(2).AddNode(ifBlockNode));

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.ShouldContainSingle<IfNode>(i =>
        {
            i.DecisionBlocks.Should().ContainSingle()
                .Which.Should().BeOfType<DecisionBlock>()
                .Which.ShouldSatisfy(i =>
                {
                    i.Condition.Should().BeSameAs(condition);
                    i.Block.ShouldContainOnly(ifBlockNode);
                });
            i.ElseBlock.ShouldBeEmpty();
            i.ChildBlock.Should().BeNull();
        });       
    }
}

