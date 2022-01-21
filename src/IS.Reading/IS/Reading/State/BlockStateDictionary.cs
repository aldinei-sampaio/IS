namespace IS.Reading.State;

public class BlockStateDictionary : IBlockStateDictionary
{
    private readonly Dictionary<int, IBlockState> dic = new();
    private readonly IBlockStateFactory blockStateFactory;

    public BlockStateDictionary(IBlockStateFactory blockStateFactory)
        => this.blockStateFactory = blockStateFactory;

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
