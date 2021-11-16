namespace IS.Reading.Navigation.BlockNavigatorTests;

public class EnterLeaveTests
{
    [Fact]
    public async Task EmptyBlock()
    {
        var tester = new BackAndForthTester();
        await CheckEmptyBlockAsync(tester);
    }

    private static async Task CheckEmptyBlockAsync(BackAndForthTester tester)
    {
        await tester.ForwardAsync(null);
        tester.CheckLog();
        await tester.BackwardAsync(null);
        tester.CheckLog();
    }

    [Fact]
    public async Task SingleNode()
    {
        var tester = new BackAndForthTester();
        tester.AddLoggedNode("normal", "reversed");
        await CheckSingleNodeAsync(tester);
    }

    private static async Task CheckSingleNodeAsync(BackAndForthTester tester)
    {
        await tester.ForwardAsync("normal");
        tester.CheckLog("Enter:normal");

        await tester.ForwardAsync(null);
        tester.CheckLog("Leave:normal");

        await tester.BackwardAsync("reversed");
        tester.CheckLog("Enter:reversed");

        await tester.BackwardAsync(null);
        tester.CheckLog("Leave:reversed");

        await tester.ForwardAsync("normal");
        tester.CheckLog("Enter:normal");

        await tester.BackwardAsync("reversed");
        tester.CheckLog("Leave:normal", "Enter:reversed");

        await tester.ForwardAsync("normal");
        tester.CheckLog("Leave:reversed", "Enter:normal");
    }

    [Fact]
    public async Task Condition_SingleNode()
    {
        var tester = new BackAndForthTester();
        var isValid = false;
        tester.AddLoggedNode("normal", "reversed", () => isValid);

        isValid = false;

        await CheckEmptyBlockAsync(tester);

        isValid = true;

        await CheckSingleNodeAsync(tester);
    }

    [Fact]
    public async Task BackAndForth()
    {
        var tester = new BackAndForthTester();
        tester.AddLoggedNode("n1", "r1");
        tester.AddLoggedNode("n2", "r2");
        tester.AddLoggedNode("n3", "r3");

        // Forward
        await tester.ForwardAsync("n1");
        tester.CheckLog("Enter:n1");

        await tester.BackwardAsync("r1", null);
        tester.CheckLog("Leave:n1", "Enter:r1", "Leave:r1");

        await tester.ForwardAsync("n1", "n2");
        tester.CheckLog("Enter:n1", "Leave:n1", "Enter:n2");

        await tester.BackwardAsync("r2", "r1", null);
        tester.CheckLog("Leave:n2", "Enter:r2", "Leave:r2", "Enter:r1", "Leave:r1");

        await tester.ForwardAsync("n1", "n2", "n3");
        tester.CheckLog("Enter:n1", "Leave:n1", "Enter:n2", "Leave:n2", "Enter:n3");

        await tester.BackwardAsync("r3", "r2", "r1", null);
        tester.CheckLog("Leave:n3", "Enter:r3", "Leave:r3", "Enter:r2", "Leave:r2", "Enter:r1", "Leave:r1");

        await tester.ForwardAsync("n1", "n2", "n3", null);
        tester.CheckLog("Enter:n1", "Leave:n1", "Enter:n2", "Leave:n2", "Enter:n3", "Leave:n3");

        // Backward
        await tester.BackwardAsync("r3");
        tester.CheckLog("Enter:r3");

        await tester.ForwardAsync("n3", null);
        tester.CheckLog("Leave:r3", "Enter:n3", "Leave:n3");

        await tester.BackwardAsync("r3", "r2");
        tester.CheckLog("Enter:r3", "Leave:r3", "Enter:r2");

        await tester.ForwardAsync("n2", "n3", null);
        tester.CheckLog("Leave:r2", "Enter:n2", "Leave:n2", "Enter:n3", "Leave:n3");

        await tester.BackwardAsync("r3", "r2", "r1");
        tester.CheckLog("Enter:r3", "Leave:r3", "Enter:r2", "Leave:r2", "Enter:r1");

        await tester.ForwardAsync("n1", "n2", "n3", null);
        tester.CheckLog("Leave:r1", "Enter:n1", "Leave:n1", "Enter:n2", "Leave:n2", "Enter:n3", "Leave:n3");

        await tester.BackwardAsync("r3", "r2", "r1", null);
        tester.CheckLog("Enter:r3", "Leave:r3", "Enter:r2", "Leave:r2", "Enter:r1", "Leave:r1");
    }
}
