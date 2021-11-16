namespace IS.Reading.Navigation.BlockNavigatorTests;

public class ConditionTests
{
    [Fact]
    public async void SingleNode()
    {
        var tester = new BackAndForthTester();
        var isValid = false;        
        tester.AddNode("normal", "reversed", () => isValid);

        isValid = false;
        
        await tester.ForwardAsync(null);
        await tester.BackwardAsync(null);

        isValid = true;

        await tester.ForwardAsync("normal", null);
        await tester.BackwardAsync("reversed", null);
    }

    [Fact]
    public async void ConditionsShouldNotAffectBackwardsMotion()
    {
        var tester = new BackAndForthTester();
        var isValid = false;
        tester.AddNode("normal", "reversed", () => isValid);

        isValid = false;

        await tester.ForwardAsync(null);
        isValid = true;
        await tester.BackwardAsync(null); // Unaffected by condition
        await tester.ForwardAsync("normal", null);
        isValid = false;
        await tester.BackwardAsync("reversed", null); // Unaffected by condition
    }

    [Fact]
    public async void SingleNode_BackAndForth()
    {
        var tester = new BackAndForthTester();
        var isValid = false;
        tester.AddNode("normal", "reversed", () => isValid);

        for (var n = 1; n <= 3; n++)
        {
            isValid = false;

            await tester.ForwardAsync(null);
            await tester.BackwardAsync(null);

            isValid = true;

            await tester.ForwardAsync("normal");
            await tester.BackwardAsync("reversed", null);
            await tester.ForwardAsync("normal", null);

            await tester.BackwardAsync("reversed");
            await tester.ForwardAsync("normal", null);
            await tester.BackwardAsync("reversed", null);
        }
    }

    [Fact]
    public async void TwoNodes()
    {
        var tester = new BackAndForthTester();
        var isValid = false;
        tester.AddNode("n1", "r1", () => isValid);
        tester.AddNode("n2", "r2");

        for (var n = 1; n <= 3; n++)
        {
            isValid = false;

            await tester.ForwardAsync("n2", null);
            await tester.BackwardAsync("r2", null);

            isValid = true;

            await tester.ForwardAsync("n1", "n2", null);
            await tester.BackwardAsync("r2", "r1", null);
        }
    }

    [Fact]
    public async void TwoNodes_BackAndForth()
    {
        var tester = new BackAndForthTester();
        var isValid = false;
        tester.AddNode("n1", "r1");
        tester.AddNode("n2", "r2", () => isValid);

        for (var n = 1; n <= 3; n++)
        {
            isValid = false;

            await tester.ForwardAsync("n1", null);
            await tester.BackwardAsync("r1", null);

            isValid = true;

            await tester.ForwardAsync("n1");
            await tester.BackwardAsync("r1", null);
            await tester.ForwardAsync("n1", "n2");
            await tester.BackwardAsync("r2", "r1", null);
            await tester.ForwardAsync("n1", "n2", null);

            await tester.BackwardAsync("r2");
            await tester.ForwardAsync("n2", null);
            await tester.BackwardAsync("r2", "r1");
            await tester.ForwardAsync("n1", "n2", null);
            await tester.BackwardAsync("r2", "r1", null);
        }
    }

    [Fact]
    public async void ThreeNodes()
    {
        var tester = new BackAndForthTester();
        var isValid1 = false;
        tester.AddNode("n1", "r1", () => isValid1);
        var isValid2 = false;
        tester.AddNode("n2", "r2", () => isValid2);
        var isValid3 = false;
        tester.AddNode("n3", "r3", () => isValid3);

        for (var n = 1; n <= 3; n++)
        {
            isValid1 = isValid2 = isValid3 = false;

            await tester.ForwardAsync(null);
            await tester.BackwardAsync(null);

            isValid1 = true;

            await tester.ForwardAsync("n1", null);
            await tester.BackwardAsync("r1", null);

            isValid2 = true;

            await tester.ForwardAsync("n1", "n2", null);
            await tester.BackwardAsync("r2", "r1", null);

            isValid3 = true;

            await tester.ForwardAsync("n1", "n2", "n3", null);
            await tester.BackwardAsync("r3", "r2", "r1", null);

            isValid2 = false;

            await tester.ForwardAsync("n1", "n3", null);
            await tester.BackwardAsync("r3", "r1", null);

            isValid1 = false;

            await tester.ForwardAsync("n3", null);
            await tester.BackwardAsync("r3", null);

            isValid2 = true;
            isValid3 = true;

            await tester.ForwardAsync("n2", "n3", null);
            await tester.BackwardAsync("r3", "r2", null);
        }
    }
}
