namespace IS.Reading.State;

public interface IBlockStateDictionary
{
    IBlockState this[int blockId] { get; }
}
