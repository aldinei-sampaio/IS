using IS.Reading.Navigation;

namespace IS.Reading.State;

public class BlockIterationState : IBlockIterationState
{
    public Stack<object?> BackwardStack { get; } = new();

    public int? CurrentNodeIndex { get; set; }

    public INode? CurrentNode { get; set; }

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

    private readonly IBlockStateFactory blockStateFactory;

    public BlockIterationState(IBlockStateFactory blockStateFactory)
        => this.blockStateFactory = blockStateFactory;
}
