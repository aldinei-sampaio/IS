namespace IS.Reading.State;

public interface IBlockStateDictionary
{
    IBlockIterationState this[int blockId, int iteration] { get; }
}
