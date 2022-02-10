using IS.Reading.Conditions;
using IS.Reading.Navigation;
using IS.Reading.Variables;

namespace IS.Reading.Choices;

public class BuilderDecisionTests
{
    private readonly INavigationContext navigationContext;
    private readonly IVariableDictionary variableDictionary;
    private readonly string prototype;

    private readonly ICondition condition;
    private readonly IBuilder<string> builder1;
    private readonly IBuilder<string> builder2;
    private readonly IBuilder<string> builder3;
    private readonly IBuilder<string> builder4;
    private readonly IBuilder<string> builder5;
    private readonly IEnumerable<IBuilder<string>> ifBlock;
    private readonly IEnumerable<IBuilder<string>> elseBlock;
    private readonly BuilderDecision<string> sut;


    public BuilderDecisionTests()
    {
        variableDictionary = A.Fake<IVariableDictionary>(i => i.Strict());
        navigationContext = A.Fake<INavigationContext>(i => i.Strict());
        A.CallTo(() => navigationContext.Variables).Returns(variableDictionary);
        prototype = "Objeto que está sendo construído (não faz sentido ser string, mas isso não faz diferença para estes testes)";

        condition = A.Fake<ICondition>(i => i.Strict());

        builder1 = A.Fake<IBuilder<string>>(i => i.Strict());
        builder2 = A.Fake<IBuilder<string>>(i => i.Strict());
        ifBlock = new[] { builder1, builder2 };

        builder3 = A.Fake<IBuilder<string>>(i => i.Strict());
        builder4 = A.Fake<IBuilder<string>>(i => i.Strict());
        builder5 = A.Fake<IBuilder<string>>(i => i.Strict());
        elseBlock = new[] { builder3, builder4, builder5 };

        sut = new BuilderDecision<string>(condition, ifBlock, elseBlock);
    }

    [Fact]
    public void Initialization()
    {
        sut.Condition.Should().BeSameAs(condition);
        sut.IfBlock.Should().BeSameAs(ifBlock);
        sut.ElseBlock.Should().BeSameAs(elseBlock);
    }

    [Fact]
    public void TrueCondition()
    {
        A.CallTo(() => condition.Evaluate(variableDictionary)).Returns(true);

        A.CallTo(() => builder1.Build(prototype, navigationContext)).DoesNothing();
        A.CallTo(() => builder2.Build(prototype, navigationContext)).DoesNothing();

        sut.Build(prototype, navigationContext);

        A.CallTo(() => builder1.Build(prototype, navigationContext)).MustHaveHappenedOnceExactly();
        A.CallTo(() => builder2.Build(prototype, navigationContext)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void FalseCondition()
    {
        A.CallTo(() => condition.Evaluate(variableDictionary)).Returns(false);

        A.CallTo(() => builder3.Build(prototype, navigationContext)).DoesNothing();
        A.CallTo(() => builder4.Build(prototype, navigationContext)).DoesNothing();
        A.CallTo(() => builder5.Build(prototype, navigationContext)).DoesNothing();

        sut.Build(prototype, navigationContext);

        A.CallTo(() => builder3.Build(prototype, navigationContext)).MustHaveHappenedOnceExactly();
        A.CallTo(() => builder4.Build(prototype, navigationContext)).MustHaveHappenedOnceExactly();
        A.CallTo(() => builder5.Build(prototype, navigationContext)).MustHaveHappenedOnceExactly();
    }
}
