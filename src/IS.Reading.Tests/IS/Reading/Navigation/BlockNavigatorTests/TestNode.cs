using IS.Reading.Conditions;

namespace IS.Reading.Navigation.BlockNavigatorTests;

public class TestNode : INode
{
    private readonly List<string> log;
    private readonly string reversedName;

    public TestNode(string name, string reversedName, ICondition condition, List<string> log) 
        : this(name, condition, log)
    {
        this.reversedName = reversedName;
    }

    private TestNode(string name, ICondition condition, List<string> log)
    {
        Name = name;
        When = condition;
        this.log = log;
    }

    public string Name { get; }

    public ICondition When { get; }

    public ICondition While { get; }

    public IBlock ChildBlock => null;

    public string LastEnteredName { get; private set; }

    public Task<object> EnterAsync(INavigationContext context)
    {
        LastEnteredName = Name;
        log?.Add($"Enter:{Name}");
        return Task.FromResult((object)reversedName);
    }

    public Task EnterAsync(INavigationContext context, object state)
    {
        LastEnteredName = (string)state;
        log?.Add($"Enter:{state}");
        return Task.CompletedTask;
    }

    public Task LeaveAsync(INavigationContext context)
    {
        log?.Add($"Leave:{LastEnteredName}");
        return Task.CompletedTask;
    }
}
