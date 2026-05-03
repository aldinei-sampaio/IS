namespace IS.Reading.State;

public class BlockState : IBlockState
{
    private readonly Stack<IBlockIterationState> iterations = new();
    private readonly IBlockStateFactory blockStateFactory;

    public BlockState(IBlockStateFactory blockStateFactory)
        => this.blockStateFactory = blockStateFactory;

    public IBlockIterationState GetCurrentIteration()
    {
        if (iterations.TryPeek(out var iteration))
            return iteration;

        MoveToNextIteration();
        return iterations.Peek();
    }

    public void MoveToNextIteration()
        => iterations.Push(blockStateFactory.CreateIterationState());

    public bool MoveToPreviousIteration()
    {
        if (!iterations.TryPop(out _))
            return false;

        return iterations.Count > 0;
    }
}
