using IS.Reading.State;

namespace IS.Reading.Navigation.BlockNavigatorTests;

public class FakeBlockState : IBlockState
{
    private readonly List<IBlockIterationState> iterations;

    public FakeBlockState()
        => iterations = new List<IBlockIterationState>() { A.Dummy<IBlockIterationState>() };

    public IBlockIterationState GetCurrentIteration()
        => iterations[^1];

    public void MoveToNextIteration()
        => iterations.Add(A.Dummy<IBlockIterationState>());

    public bool MoveToPreviousIteration()
    {
        if (iterations.Count == 1)
            return false;
        iterations.RemoveAt(iterations.Count - 1);
        return true;
    }
}

