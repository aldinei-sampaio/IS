using IS.Reading.Choices;
using IS.Reading.Conditions;
using IS.Reading.Parsing.ConditionParsers;

namespace IS.Reading.Parsing.NodeParsers.ChoiceParsers;

public class BuilderIfNodeParserTests
{
    private class TestClass : BuilderIfNodeParserBase<string>
    {
        public TestClass(
            IElementParser elementParser,
            IConditionParser conditionParser,
            INodeParser nodeParser
        )
        {
            ElementParser = elementParser;
            ConditionParser = conditionParser;
            var parsers = new INodeParser[] { nodeParser };
            IfBlockSettings = new ElementParserSettings.IfBlock(parsers);
            ElseBlockSettings = new ElementParserSettings.Block(parsers);
        }
    }

    private readonly IElementParser elementParser;
    private readonly IConditionParser conditionParser;
    private readonly INodeParser nodeParser;
    private readonly TestClass sut;
    
    private readonly IDocumentReader documentReader;
    private readonly IParsingContext parsingContext;
    private readonly BuilderParentParsingContext<string> parentParsingContext;

    public BuilderIfNodeParserTests()
    {
        elementParser = A.Fake<IElementParser>(i => i.Strict(StrictFakeOptions.AllowToString));
        conditionParser = A.Fake<IConditionParser>(i => i.Strict(StrictFakeOptions.AllowToString));
        nodeParser = Helper.FakeParser<INodeParser>("test");

        sut = new(elementParser, conditionParser, nodeParser);

        documentReader = A.Fake<IDocumentReader>(i => i.Strict());
        parsingContext = Helper.FakeParsingContext();
        parentParsingContext = new();
    }

    [Fact]
    public void Initialization()
    {
        sut.Name.Should().Be("if");
        sut.IsArgumentRequired.Should().BeTrue();
        sut.ElementParser.Should().BeSameAs(elementParser);
        sut.ConditionParser.Should().BeSameAs(conditionParser);
        sut.IfBlockSettings.Should().BeOfType<ElementParserSettings.IfBlock>();
        sut.IfBlockSettings.ChildParsers.Should().BeEquivalentTo(nodeParser);
        sut.ElseBlockSettings.Should().BeOfType<ElementParserSettings.Block>();
        sut.ElseBlockSettings.ChildParsers.Should().BeEquivalentTo(nodeParser);
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
    public async Task ShouldExitOnIfParserError()
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

        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.IfBlockSettings))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ShouldExitOnElseIfParserError()
    {
        var argument = "alpha = 1";
        var parsed = A.Dummy<ICondition>();
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => conditionParser.Parse(argument)).Returns(Result.Ok(parsed));
        A.CallTo(() => parsingContext.IsSuccess).ReturnsNextFromSequence(true, false);
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.IfBlockSettings))
            .DoesNothing();
        A.CallTo(() => documentReader.Command).Returns("elseif");
        A.CallTo(() => documentReader.AtEnd).Returns(false);

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.Builders.Should().BeEmpty();

        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.IfBlockSettings))
            .MustHaveHappenedTwiceExactly();
    }

    [Fact]
    public async Task ShouldExitOnElseParserError_1()
    {
        var argument = "alpha = 1";
        var parsed = A.Dummy<ICondition>();
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => conditionParser.Parse(argument)).Returns(Result.Ok(parsed));
        A.CallTo(() => parsingContext.IsSuccess).ReturnsNextFromSequence(true, true);
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.IfBlockSettings))
            .DoesNothing();
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.ElseBlockSettings))
            .DoesNothing();
        A.CallTo(() => documentReader.Command).Returns("else");
        A.CallTo(() => documentReader.AtEnd).Returns(false);

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.Builders.Should().BeEmpty();

        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.IfBlockSettings))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.ElseBlockSettings))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ShouldExitOnElseParserError_2()
    {
        var argument = "alpha = 1";
        var parsed = A.Dummy<ICondition>();
        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => conditionParser.Parse(argument)).Returns(Result.Ok(parsed));
        A.CallTo(() => parsingContext.IsSuccess).ReturnsNextFromSequence(true, true, false);
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.IfBlockSettings))
            .DoesNothing();
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.ElseBlockSettings))
            .DoesNothing();
        A.CallTo(() => documentReader.Command).ReturnsNextFromSequence("elseif", "else");
        A.CallTo(() => documentReader.AtEnd).Returns(false);

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.Builders.Should().BeEmpty();

        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.IfBlockSettings))
            .MustHaveHappenedTwiceExactly();
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.ElseBlockSettings))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task NoBuildersParsed_1()
    {
        var argument = "alpha = 1";
        var condition = A.Dummy<ICondition>();

        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => conditionParser.Parse(argument)).Returns(Result.Ok(condition));
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.IfBlockSettings))
            .DoesNothing();
        A.CallTo(() => documentReader.Command).Returns("end");
        A.CallTo(() => documentReader.AtEnd).Returns(false);

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.Builders.Should().BeEmpty();

        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.IfBlockSettings))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task NoBuildersParsed_2()
    {
        var argument = "alpha = 1";
        var condition = A.Dummy<ICondition>();

        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => conditionParser.Parse(argument)).Returns(Result.Ok(condition));
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.IfBlockSettings))
            .DoesNothing();
        A.CallTo(() => documentReader.Command).ReturnsNextFromSequence("elseif", "end");
        A.CallTo(() => documentReader.AtEnd).Returns(false);

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.Builders.Should().BeEmpty();

        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.IfBlockSettings))
            .MustHaveHappenedTwiceExactly();
    }

    [Fact]
    public async Task NoBuildersParsed_3()
    {
        var argument = "alpha = 1";
        var condition = A.Dummy<ICondition>();

        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => conditionParser.Parse(argument)).Returns(Result.Ok(condition));
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.IfBlockSettings))
            .DoesNothing();
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.ElseBlockSettings))
            .DoesNothing();
        A.CallTo(() => documentReader.Command).ReturnsNextFromSequence("elseif", "else", "end");
        A.CallTo(() => documentReader.AtEnd).Returns(false);

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.Builders.Should().BeEmpty();

        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.IfBlockSettings))
            .MustHaveHappenedTwiceExactly();
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.ElseBlockSettings))
            .MustHaveHappenedOnceExactly();
    }


    [Fact]
    public async Task IfOnly_1()
    {
        var argument = "alpha = 1";
        var condition = A.Dummy<ICondition>();
        var builder = A.Dummy<IBuilder<string>>();

        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => conditionParser.Parse(argument)).Returns(Result.Ok(condition));
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.IfBlockSettings))
            .Invokes(i => i.GetTestContext().Builders.Add(builder));
        A.CallTo(() => documentReader.Command).Returns("end");
        A.CallTo(() => documentReader.AtEnd).Returns(false);

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.Builders.Should().ContainSingle()
            .Which.Should().BeOfType<BuilderDecision<string>>()
            .Which.ShouldSatisfy(i =>
            {
                i.Items.Should().ContainSingle()
                    .Which.ShouldSatisfy(i =>
                    {
                        i.Condition.Should().BeSameAs(condition);
                        i.Block.Should().BeEquivalentTo(builder);
                    });
                i.ElseBlock.Should().BeEmpty();
            });
    }

    [Fact]
    public async Task IfOnly_2()
    {
        var argument = "alpha = 1";
        var condition = A.Dummy<ICondition>();
        var builder = A.Dummy<IBuilder<string>>();

        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => conditionParser.Parse(argument)).Returns(Result.Ok(condition));
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.IfBlockSettings))
            .Invokes(i => i.GetTestContext().Builders.Add(builder));
        A.CallTo(() => documentReader.AtEnd).Returns(true);

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.Builders.Should().ContainSingle()
            .Which.Should().BeOfType<BuilderDecision<string>>()
            .Which.ShouldSatisfy(i =>
            {
                i.Items.Should().ContainSingle()
                    .Which.ShouldSatisfy(i =>
                    {
                        i.Condition.Should().BeSameAs(condition);
                        i.Block.Should().BeEquivalentTo(builder);
                    });
                i.ElseBlock.Should().BeEmpty();
            });
    }

    [Fact]
    public async Task IfAndElseIf()
    {
        var argument = "alpha = 1";
        var conditions = new[] { A.Dummy<ICondition>(), A.Dummy<ICondition>() };
        var nextCondition = 0;
        var builders = new[] { A.Dummy<IBuilder<string>>(), A.Dummy<IBuilder<string>>() };
        var nextBuilder = 0;

        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => conditionParser.Parse(argument))
            .ReturnsLazily(i => Result.Ok(conditions[nextCondition++]));
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.IfBlockSettings))
            .Invokes(i => i.GetTestContext().Builders.Add(builders[nextBuilder++]));
        A.CallTo(() => documentReader.Command).ReturnsNextFromSequence("elseif", "end");
        A.CallTo(() => documentReader.AtEnd).Returns(false);

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.Builders.Should().ContainSingle()
            .Which.Should().BeOfType<BuilderDecision<string>>()
            .Which.ShouldSatisfy(i =>
            {
                i.Items.Should().Contain(
                    i => i.ShouldSatisfy(i =>
                    {
                        i.Condition.Should().BeSameAs(conditions[0]);
                        i.Block.Should().BeEquivalentTo(builders[0]);
                    }),
                    i => i.ShouldSatisfy(i =>
                    {
                        i.Condition.Should().BeSameAs(conditions[1]);
                        i.Block.Should().BeEquivalentTo(builders[1]);
                    })
                );
                i.ElseBlock.Should().BeEmpty();
            });
    }

    [Fact]
    public async Task IfAndElseIfAndElse()
    {
        var argument = "alpha = 1";
        var conditions = new[] { 
            A.Dummy<ICondition>(), 
            A.Dummy<ICondition>() 
        };
        var nextCondition = 0;
        var builders = new[] { 
            A.Dummy<IBuilder<string>>(), 
            A.Dummy<IBuilder<string>>(),
            A.Dummy<IBuilder<string>>()
        };
        var nextBuilder = 0;

        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => conditionParser.Parse(argument)).ReturnsLazily(i => Result.Ok(conditions[nextCondition++]));
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.IfBlockSettings))
            .Invokes(i => i.GetTestContext().Builders.Add(builders[nextBuilder++]));
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.ElseBlockSettings))
            .Invokes(i => i.GetTestContext().Builders.Add(builders[nextBuilder++]));
        A.CallTo(() => documentReader.Command).ReturnsNextFromSequence("elseif", "else", "end");
        A.CallTo(() => documentReader.AtEnd).Returns(false);

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.Builders.Should().ContainSingle()
            .Which.Should().BeOfType<BuilderDecision<string>>()
            .Which.ShouldSatisfy(i =>
            {
                i.Items.Should().Contain(
                    i => i.ShouldSatisfy(i =>
                    {
                        i.Condition.Should().BeSameAs(conditions[0]);
                        i.Block.Should().BeEquivalentTo(builders[0]);
                    }),
                    i => i.ShouldSatisfy(i =>
                    {
                        i.Condition.Should().BeSameAs(conditions[1]);
                        i.Block.Should().BeEquivalentTo(builders[1]);
                    })
                );
                i.ElseBlock.Should().ContainSingle()
                    .Which.Should().BeEquivalentTo(builders[2]);
            });
    }

    [Fact]
    public async Task IfAndElse_1()
    {
        var argument = "alpha = 1";
        var condition = A.Dummy<ICondition>();
        var builder1 = A.Dummy<IBuilder<string>>();
        var builder2 = A.Dummy<IBuilder<string>>();

        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => conditionParser.Parse(argument)).Returns(Result.Ok(condition));
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.IfBlockSettings))
            .Invokes(i => i.GetTestContext().Builders.Add(builder1));
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.ElseBlockSettings))
            .Invokes(i => i.GetTestContext().Builders.Add(builder2));
        A.CallTo(() => documentReader.Command).ReturnsNextFromSequence("else", "end");
        A.CallTo(() => documentReader.AtEnd).Returns(false);

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.Builders.Should().ContainSingle()
            .Which.Should().BeOfType<BuilderDecision<string>>()
            .Which.ShouldSatisfy(i =>
            {
                i.Items.Should().Contain(
                    i => i.ShouldSatisfy(i =>
                    {
                        i.Condition.Should().BeSameAs(condition);
                        i.Block.Should().BeEquivalentTo(builder1);
                    })
                );
                i.ElseBlock.Should().ContainSingle()
                    .Which.Should().BeEquivalentTo(builder2);
            });
    }

    [Fact]
    public async Task IfAndElse_2()
    {
        var argument = "alpha = 1";
        var condition = A.Dummy<ICondition>();
        var builder1 = A.Dummy<IBuilder<string>>();
        var builder2 = A.Dummy<IBuilder<string>>();

        A.CallTo(() => documentReader.Argument).Returns(argument);
        A.CallTo(() => conditionParser.Parse(argument)).Returns(Result.Ok(condition));
        A.CallTo(() => parsingContext.IsSuccess).Returns(true);
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.IfBlockSettings))
            .Invokes(i => i.GetTestContext().Builders.Add(builder1));
        A.CallTo(() => elementParser.ParseAsync(documentReader, parsingContext, A<IParentParsingContext>.Ignored, sut.ElseBlockSettings))
            .Invokes(i => i.GetTestContext().Builders.Add(builder2));
        A.CallTo(() => documentReader.Command).ReturnsNextFromSequence("else");
        A.CallTo(() => documentReader.AtEnd).ReturnsNextFromSequence(false, true);

        await sut.ParseAsync(documentReader, parsingContext, parentParsingContext);

        parentParsingContext.Builders.Should().ContainSingle()
            .Which.Should().BeOfType<BuilderDecision<string>>()
            .Which.ShouldSatisfy(i =>
            {
                i.Items.Should().Contain(
                    i => i.ShouldSatisfy(i =>
                    {
                        i.Condition.Should().BeSameAs(condition);
                        i.Block.Should().BeEquivalentTo(builder1);
                    })
                );
                i.ElseBlock.Should().ContainSingle()
                    .Which.Should().BeEquivalentTo(builder2);
            });
    }
}
