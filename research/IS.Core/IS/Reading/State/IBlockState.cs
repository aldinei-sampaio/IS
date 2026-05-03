namespace IS.Reading.State;

public interface IBlockState
{
    IBlockIterationState GetCurrentIteration();
    void MoveToNextIteration();
    bool MoveToPreviousIteration();
}
