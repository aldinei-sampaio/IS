using System.Collections.Generic;

namespace IS.Reading.Navigation
{
    public interface IStoryboard
    {
        IBlock RootBlock { get; }
        Stack<IBlock> EnteredBlocks { get; }
        IBlock? CurrentBlock { get; set; }
        INode? CurrentNode { get; set; }
    }
}
