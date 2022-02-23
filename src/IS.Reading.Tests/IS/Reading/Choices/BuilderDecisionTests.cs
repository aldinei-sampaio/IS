using IS.Reading.Conditions;
using IS.Reading.Navigation;
using IS.Reading.Variables;

namespace IS.Reading.Choices;

public class BuilderDecisionTests
{
    private class TestStruct
    {
        public ICondition Condition1 { get; }
        public ICondition Condition2 { get; }
        public ICondition Condition3 { get; }
        public IBuilder<string> Builder1 { get; }
        public IBuilder<string> Builder2 { get; }
        public IBuilder<string> Builder3 { get; }
        public IBuilderDecisionItem<string> BuilderDecisionItem1 { get; }
        public IBuilderDecisionItem<string> BuilderDecisionItem2 { get; }
        public IBuilderDecisionItem<string> BuilderDecisionItem3 { get; }
        public IEnumerable<IBuilderDecisionItem<string>> IfItems { get; }
        public IBuilder<string> ElseBuilder { get; }
        public IEnumerable<IBuilder<string>> ElseItems { get; }

        public TestStruct()
        {
            Condition1 = A.Fake<ICondition>(i => i.Strict());
            Condition2 = A.Fake<ICondition>(i => i.Strict());
            Condition3 = A.Fake<ICondition>(i => i.Strict());

            Builder1 = A.Fake<IBuilder<string>>(i => i.Strict());
            Builder2 = A.Fake<IBuilder<string>>(i => i.Strict());
            Builder3 = A.Fake<IBuilder<string>>(i => i.Strict());

            BuilderDecisionItem1 = A.Fake<IBuilderDecisionItem<string>>(i => i.Strict());
            BuilderDecisionItem2 = A.Fake<IBuilderDecisionItem<string>>(i => i.Strict());
            BuilderDecisionItem3 = A.Fake<IBuilderDecisionItem<string>>(i => i.Strict());

            A.CallTo(() => BuilderDecisionItem1.Condition).Returns(Condition1);
            A.CallTo(() => BuilderDecisionItem2.Condition).Returns(Condition2);
            A.CallTo(() => BuilderDecisionItem3.Condition).Returns(Condition3);

            A.CallTo(() => BuilderDecisionItem1.Block).Returns(new[] { Builder1 });
            A.CallTo(() => BuilderDecisionItem2.Block).Returns(new[] { Builder2 });
            A.CallTo(() => BuilderDecisionItem3.Block).Returns(new[] { Builder3 });

            IfItems = new[] { BuilderDecisionItem1, BuilderDecisionItem2, BuilderDecisionItem3 };
            
            ElseBuilder = A.Fake<IBuilder<string>>(i => i.Strict());
            ElseItems = new[] { ElseBuilder };
        }

        public BuilderDecision<string> CreateSut()
            => new(IfItems, ElseItems);
    }

    private readonly INavigationContext navigationContext;
    private readonly IVariableDictionary variableDictionary;
    private readonly string prototype;

    private readonly TestStruct struc;
    private readonly BuilderDecision<string> sut;

    public BuilderDecisionTests()
    {
        variableDictionary = A.Fake<IVariableDictionary>(i => i.Strict());
        navigationContext = A.Fake<INavigationContext>(i => i.Strict());
        A.CallTo(() => navigationContext.Variables).Returns(variableDictionary);
        prototype = "Objeto que está sendo construído (não faz sentido ser string, mas isso não faz diferença para estes testes)";

        struc = new();
        sut = struc.CreateSut();
    }

    [Fact]
    public void Initialization()
    {
        sut.Items.Should().BeSameAs(struc.IfItems);
        sut.ElseBlock.Should().BeSameAs(struc.ElseItems);
    }

    [Fact]
    public void FirstConditionTrue()
    {
        A.CallTo(() => struc.Condition1.Evaluate(variableDictionary)).Returns(true);
        A.CallTo(() => struc.Builder1.Build(prototype, navigationContext)).DoesNothing();

        sut.Build(prototype, navigationContext);

        A.CallTo(() => struc.Condition1.Evaluate(variableDictionary)).MustHaveHappenedOnceExactly();
        A.CallTo(() => struc.Builder1.Build(prototype, navigationContext)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void SecondConditionTrue()
    {
        A.CallTo(() => struc.Condition1.Evaluate(variableDictionary)).Returns(false);
        A.CallTo(() => struc.Condition2.Evaluate(variableDictionary)).Returns(true);
        A.CallTo(() => struc.Builder2.Build(prototype, navigationContext)).DoesNothing();

        sut.Build(prototype, navigationContext);

        A.CallTo(() => struc.Condition1.Evaluate(variableDictionary)).MustHaveHappenedOnceExactly();
        A.CallTo(() => struc.Condition2.Evaluate(variableDictionary)).MustHaveHappenedOnceExactly();
        A.CallTo(() => struc.Builder2.Build(prototype, navigationContext)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void ThirdConditionTrue()
    {
        A.CallTo(() => struc.Condition1.Evaluate(variableDictionary)).Returns(false);
        A.CallTo(() => struc.Condition2.Evaluate(variableDictionary)).Returns(false);
        A.CallTo(() => struc.Condition3.Evaluate(variableDictionary)).Returns(true);
        A.CallTo(() => struc.Builder3.Build(prototype, navigationContext)).DoesNothing();

        sut.Build(prototype, navigationContext);

        A.CallTo(() => struc.Condition1.Evaluate(variableDictionary)).MustHaveHappenedOnceExactly();
        A.CallTo(() => struc.Condition2.Evaluate(variableDictionary)).MustHaveHappenedOnceExactly();
        A.CallTo(() => struc.Condition3.Evaluate(variableDictionary)).MustHaveHappenedOnceExactly();
        A.CallTo(() => struc.Builder3.Build(prototype, navigationContext)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void AllConditionsFalse()
    {
        A.CallTo(() => struc.Condition1.Evaluate(variableDictionary)).Returns(false);
        A.CallTo(() => struc.Condition2.Evaluate(variableDictionary)).Returns(false);
        A.CallTo(() => struc.Condition3.Evaluate(variableDictionary)).Returns(false);
        A.CallTo(() => struc.ElseBuilder.Build(prototype, navigationContext)).DoesNothing();

        sut.Build(prototype, navigationContext);

        A.CallTo(() => struc.Condition1.Evaluate(variableDictionary)).MustHaveHappenedOnceExactly();
        A.CallTo(() => struc.Condition2.Evaluate(variableDictionary)).MustHaveHappenedOnceExactly();
        A.CallTo(() => struc.Condition3.Evaluate(variableDictionary)).MustHaveHappenedOnceExactly();
        A.CallTo(() => struc.ElseBuilder.Build(prototype, navigationContext)).MustHaveHappenedOnceExactly();
    }
}
