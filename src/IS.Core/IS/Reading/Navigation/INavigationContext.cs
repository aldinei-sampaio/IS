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
    IBlock? CurrentBlock { get; set; }
    INode? CurrentNode { get; set; }
    IRandomizer Randomizer { get; }
    int CurrentIteration { get; set; }
}