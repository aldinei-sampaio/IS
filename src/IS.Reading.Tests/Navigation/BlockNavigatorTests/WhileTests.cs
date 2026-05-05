using IS.Reading.Conditions;

namespace IS.Reading.Navigation.BlockNavigatorTests;

public class WhileTests
{
    [Fact]
    public async Task RepeatSingleNode()
    {
        var tester = new NavigatorTester();

        var @while = A.Fake<ICondition>(i => i.Strict());
        A.CallTo(() => @while.Evaluate(tester.Context.Variables))
            .ReturnsNextFromSequence(true, true, false);

        A.CallTo(() => tester.Block.While).Returns(@while);

        tester.AddNode("N1", "R1");

        await tester.MoveAsync(true, "N1", "N1", "N1", null);
        await tester.MoveAsync(false, "R1", "R1", "R1", null);
    }

    [Fact]
    public async Task RepeatMultipleNodes()
    {
        var tester = new NavigatorTester();

        var @while = A.Fake<ICondition>(i => i.Strict());
        A.CallTo(() => @while.Evaluate(tester.Context.Variables))
            .ReturnsNextFromSequence(true, false);

        A.CallTo(() => tester.Block.While).Returns(@while);

        tester.AddNode("N1", "R1");
        tester.AddNode("N2", "R2");
        tester.AddNode("N3", "R3");

        await tester.MoveAsync(true, "N1", "N2", "N3", "N1", "N2", "N3", null);
        await tester.MoveAsync(false, "R3", "R2", "R1", "R3", "R2", "R1", null);
    }

    [Fact]
    public async Task IgnoreNodeWhenChildBlockWhileReturnsFalse()
    {
        var tester = new NavigatorTester();

        var @while = A.Fake<ICondition>(i => i.Strict());
        A.CallTo(() => @while.Evaluate(tester.Context.Variables))
            .Returns(false);

        tester.AddNode("N1", "R1");

        tester.AddNode("N2", "R2").ConfigureChildBlock(
            i => A.CallTo(() => i.While).Returns(@while)
        );

        tester.AddNode("N3", "R3");

        await tester.MoveAsync(true, "N1", "N3", null);
        await tester.MoveAsync(false, "R3", "R1", null);
    }
}
