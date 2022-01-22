namespace IS.Reading.Navigation.BlockNavigatorTests;

public class ConditionTests
{
    [Fact]
    public async void SingleNode()
    {
        var tester = new NavigatorTester();
        var isValid = false;        
        tester.AddNode("normal", "reversed", () => isValid);

        isValid = false;
        
        await tester.MoveAsync(true, null);
        await tester.MoveAsync(false, null);

        isValid = true;

        await tester.MoveAsync(true, "normal", null);
        await tester.MoveAsync(false, "reversed", null);
    }

    [Fact]
    public async void ConditionsShouldNotAffectBackwardsMotion()
    {
        var tester = new NavigatorTester();
        var isValid = false;
        tester.AddNode("normal", "reversed", () => isValid);

        isValid = false;

        await tester.MoveAsync(true, null);
        isValid = true;
        await tester.MoveAsync(false, null); // Unaffected by condition
        await tester.MoveAsync(true, "normal", null);
        isValid = false;
        await tester.MoveAsync(false, "reversed", null); // Unaffected by condition
    }

    [Fact]
    public async void SingleNode_BackAndForth()
    {
        var tester = new NavigatorTester();
        var isValid = false;
        tester.AddNode("normal", "reversed", () => isValid);

        for (var n = 1; n <= 3; n++)
        {
            isValid = false;

            await tester.MoveAsync(true, null);
            await tester.MoveAsync(false, null);

            isValid = true;

            await tester.MoveAsync(true, "normal");
            await tester.MoveAsync(false, "reversed", null);
            await tester.MoveAsync(true, "normal", null);

            await tester.MoveAsync(false, "reversed");
            await tester.MoveAsync(true, "normal", null);
            await tester.MoveAsync(false, "reversed", null);
        }
    }

    [Fact]
    public async void TwoNodes()
    {
        var tester = new NavigatorTester();
        var isValid = false;
        tester.AddNode("n1", "r1", () => isValid);
        tester.AddNode("n2", "r2");

        for (var n = 1; n <= 3; n++)
        {
            isValid = false;

            await tester.MoveAsync(true, "n2", null);
            await tester.MoveAsync(false, "r2", null);

            isValid = true;

            await tester.MoveAsync(true, "n1", "n2", null);
            await tester.MoveAsync(false, "r2", "r1", null);
        }
    }

    [Fact]
    public async void TwoNodes_BackAndForth()
    {
        var tester = new NavigatorTester();
        var isValid = false;
        tester.AddNode("n1", "r1");
        tester.AddNode("n2", "r2", () => isValid);

        for (var n = 1; n <= 3; n++)
        {
            isValid = false;

            await tester.MoveAsync(true, "n1", null);
            await tester.MoveAsync(false, "r1", null);

            isValid = true;

            await tester.MoveAsync(true, "n1");
            await tester.MoveAsync(false, "r1", null);
            await tester.MoveAsync(true, "n1", "n2");
            await tester.MoveAsync(false, "r2", "r1", null);
            await tester.MoveAsync(true, "n1", "n2", null);

            await tester.MoveAsync(false, "r2");
            await tester.MoveAsync(true, "n2", null);
            await tester.MoveAsync(false, "r2", "r1");
            await tester.MoveAsync(true, "n1", "n2", null);
            await tester.MoveAsync(false, "r2", "r1", null);
        }
    }

    [Fact]
    public async void ThreeNodes()
    {
        var tester = new NavigatorTester();
        var isValid1 = false;
        tester.AddNode("n1", "r1", () => isValid1);
        var isValid2 = false;
        tester.AddNode("n2", "r2", () => isValid2);
        var isValid3 = false;
        tester.AddNode("n3", "r3", () => isValid3);

        for (var n = 1; n <= 3; n++)
        {
            isValid1 = isValid2 = isValid3 = false;

            await tester.MoveAsync(true, null);
            await tester.MoveAsync(false, null);

            isValid1 = true;

            await tester.MoveAsync(true, "n1", null);
            await tester.MoveAsync(false, "r1", null);

            isValid2 = true;

            await tester.MoveAsync(true, "n1", "n2", null);
            await tester.MoveAsync(false, "r2", "r1", null);

            isValid3 = true;

            await tester.MoveAsync(true, "n1", "n2", "n3", null);
            await tester.MoveAsync(false, "r3", "r2", "r1", null);

            isValid2 = false;

            await tester.MoveAsync(true, "n1", "n3", null);
            await tester.MoveAsync(false, "r3", "r1", null);

            isValid1 = false;

            await tester.MoveAsync(true, "n3", null);
            await tester.MoveAsync(false, "r3", null);

            isValid2 = true;
            isValid3 = true;

            await tester.MoveAsync(true, "n2", "n3", null);
            await tester.MoveAsync(false, "r3", "r2", null);
        }
    }
}
