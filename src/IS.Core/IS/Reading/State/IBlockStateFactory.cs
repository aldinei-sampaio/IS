namespace IS.Reading.State;

public interface IBlockStateFactory
{
    IBlockIterationState CreateIterationState();
    IBlockState CreateState();
    IBlockStateDictionary CreateStateDictionary();
}
