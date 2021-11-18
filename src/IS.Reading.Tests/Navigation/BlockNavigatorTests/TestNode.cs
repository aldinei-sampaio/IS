namespace IS.Reading.Navigation.BlockNavigatorTests;

public class TestNode : INode
{
    private readonly List<string> log;

    public TestNode(string name, string reversedName, ICondition condition, List<string> log) 
        : this(name, condition, log)
    {
        var reversed = new TestNode(reversedName, null, log);
        reversed.Reversed = this;
        Reversed = reversed;
    }

    private TestNode(string name, ICondition condition, List<string> log)
    {
        Name = name;
        Condition = condition;
        this.log = log;
    }

    public string Name { get; }

    public INode Reversed { get; internal set; }

    public ICondition Condition { get; }

    public IBlock ChildBlock => null;

    public Task<INode> EnterAsync(IContext context)
    {
        log?.Add($"Enter:{Name}");
        return Task.FromResult(Reversed);
    }

    public Task LeaveAsync(IContext context)
    {
        log?.Add($"Leave:{Name}");
        return Task.CompletedTask;
    }
}
