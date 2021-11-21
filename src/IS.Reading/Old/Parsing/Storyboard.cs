using IS.Reading.Navigation;
using System.Collections.Generic;

namespace IS.Reading.Parsing;

public class Storyboard : IStoryboard
{
    public IBlock RootBlock { get; }

    public Stack<IBlock> EnteredBlocks { get; } = new();

    public IBlock? CurrentBlock { get; set; }
    public INode? CurrentNode { get; set; }

    public Storyboard(IBlock rootBlock)
    {
        RootBlock = rootBlock;
    }
}
