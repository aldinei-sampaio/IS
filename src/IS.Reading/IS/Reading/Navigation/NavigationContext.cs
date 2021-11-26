using IS.Reading.Events;
using IS.Reading.State;

namespace IS.Reading.Navigation;

public class NavigationContext : INavigationContext
{
    public INavigationState State { get; } = new NavigationState();

    public IEventInvoker Events { get; }

    public IBlock RootBlock { get; }

    public Stack<IBlock> EnteredBlocks { get; } = new();

    public IBlock? CurrentBlock { get; set; }

    public INode? CurrentNode { get; set; }

    public IVariableDictionary Variables { get; } = new VariableDictionary();

    public NavigationContext(IBlock rootBlock, IEventInvoker events)
    {
        RootBlock = rootBlock;
        Events = events;
    }
}
