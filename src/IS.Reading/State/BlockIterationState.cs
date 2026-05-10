using IS.Reading.Navigation;

namespace IS.Reading.State;

public class BlockIterationState(IBlockStateFactory blockStateFactory) : IBlockIterationState
{
    public Stack<object?> BackwardStack { get; } = new();

    public int? CurrentNodeIndex { get; set; }

    public INode? CurrentNode { get; set; }

    public bool SkipFirstPause { get; set; }

    private IBlockStateDictionary? children;

    public IBlockStateDictionary Children
    {
        get
        {
            if (children is null)
                children = blockStateFactory.CreateStateDictionary();
            return children;
        }
    }
}
