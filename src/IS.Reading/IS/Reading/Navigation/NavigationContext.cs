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

    public Stack<IBlockState> EnteredBlockStates { get; } = new();

    public IBlock? CurrentBlock { get; set; }

    public IBlockState? CurrentBlockState { get; set; }

    public INode? CurrentNode { get; set; }

    public IVariableDictionary Variables { get; }

    public IRandomizer Randomizer { get; }

    public IBlockState RootBlockState { get; }

    public NavigationContext(
        IBlock rootBlock, 
        IBlockState rootBlockState,
        IEventInvoker events, 
        IRandomizer randomizer, 
        INavigationState navigationState,
        IVariableDictionary variableDictionary
    )
    {
        RootBlock = rootBlock;
        RootBlockState = rootBlockState;
        Events = events;
        Randomizer = randomizer;
        State = navigationState;
        Variables = variableDictionary;
    }
}
