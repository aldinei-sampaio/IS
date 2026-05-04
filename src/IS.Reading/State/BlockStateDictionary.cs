namespace IS.Reading.State;

public class BlockStateDictionary(IBlockStateFactory blockStateFactory) : IBlockStateDictionary
{
    private readonly Dictionary<int, IBlockState> dic = new();

    public IBlockState this[int blockId]
    {
        get
        {
            if (blockId < 0)
                throw new ArgumentOutOfRangeException(nameof(blockId));

            if (dic.TryGetValue(blockId, out var state))
                return state;

            state = blockStateFactory.CreateState();
            dic[blockId] = state;
            return state;
        }
    }
}
