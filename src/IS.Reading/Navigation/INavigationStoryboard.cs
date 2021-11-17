using System.Collections.Generic;

namespace IS.Reading.Navigation
{
    public interface INavigationStoryboard
    {
        INavigationBlock RootBlock { get; }
        Stack<INavigationBlock> EnteredBlocks { get; }
        INavigationBlock? CurrentBlock { get; set; }
        INavigationNode? CurrentNode { get; set; }
    }
}
