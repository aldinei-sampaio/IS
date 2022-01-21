using IS.Reading.Events;
using IS.Reading.State;
using IS.Reading.Variables;

namespace IS.Reading.Navigation;

public class NavigationContext : INavigationContext
{
    public INavigationState State { get; }

    public IEventInvoker Events { get; }

    public IBlock RootBlock { get; }

    public Stack<IBlock> EnteredBlocks { get; } = new();

    public IBlock? CurrentBlock { get; set; }

    public INode? CurrentNode { get; set; }

    public IVariableDictionary Variables { get; }

    public IRandomizer Randomizer { get; }

    public int CurrentIteration { get; set; }

    public NavigationContext(
        IBlock rootBlock, 
        IEventInvoker events, 
        IRandomizer randomizer, 
        INavigationState navigationState,
        IVariableDictionary variableDictionary
    )
    {
        RootBlock = rootBlock;
        Events = events;
        Randomizer = randomizer;
        State = navigationState;
        Variables = variableDictionary;
    }
}
