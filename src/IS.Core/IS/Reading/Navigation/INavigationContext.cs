using IS.Reading.Events;
using IS.Reading.State;
using IS.Reading.Variables;

namespace IS.Reading.Navigation;

public interface INavigationContext
{
    IVariableDictionary Variables { get; }
    INavigationState State { get; }
    IEventInvoker Events { get; }
    IBlock RootBlock { get; }
    Stack<IBlock> EnteredBlocks { get; }
    Stack<IBlockState> EnteredBlockStates { get; }
    IBlock? CurrentBlock { get; set; }
    IBlockState? CurrentBlockState { get; set; }
    INode? CurrentNode { get; set; }
    IRandomizer Randomizer { get; }
    IBlockState RootBlockState { get; }
}