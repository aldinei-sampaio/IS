using IS.Reading.Events;
using IS.Reading.State;
using IS.Reading.Variables;

namespace IS.Reading.Navigation;

public class NavigationContext(
    IBlock rootBlock,
    IBlockState rootBlockState,
    IEventInvoker events,
    IRandomizer randomizer,
    INavigationState navigationState,
    IVariableDictionary variableDictionary
) : INavigationContext
{
    public INavigationState State { get; } = navigationState;

    public IEventInvoker Events { get; } = events;

    public IBlock RootBlock { get; } = rootBlock;

    public Stack<IBlock> EnteredBlocks { get; } = new();

    public Stack<IBlockState> EnteredBlockStates { get; } = new();

    public IBlock? CurrentBlock { get; set; }

    public IBlockState? CurrentBlockState { get; set; }

    public INode? CurrentNode { get; set; }

    public IVariableDictionary Variables { get; } = variableDictionary;

    public IRandomizer Randomizer { get; } = randomizer;

    public IBlockState RootBlockState { get; } = rootBlockState;
}
