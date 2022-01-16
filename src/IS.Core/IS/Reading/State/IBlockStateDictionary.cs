namespace IS.Reading.State;

public interface IBlockStateDictionary
{
    IBlockState this[int blockId, int iteration] { get; }
}
