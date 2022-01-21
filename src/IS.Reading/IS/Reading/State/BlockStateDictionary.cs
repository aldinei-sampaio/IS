namespace IS.Reading.State;

public class BlockStateDictionary : IBlockStateDictionary
{
    private readonly Dictionary<int, List<IBlockIterationState>> dic = new();
    private readonly IBlockStateFactory blockStateFactory;

    public BlockStateDictionary(IBlockStateFactory blockStateFactory)
        => this.blockStateFactory = blockStateFactory;

    public IBlockIterationState this[int blockId, int iteration]
    {
        get
        {
            if (blockId < 0)
                throw new ArgumentOutOfRangeException(nameof(blockId));

            if (iteration < 0)
                throw new ArgumentOutOfRangeException(nameof(iteration));

            List<IBlockIterationState>? list = null;

            lock (dic) {
                if (!dic.TryGetValue(blockId, out list))
                {
                    list = new List<IBlockIterationState>();
                    dic[blockId] = list;
                }
            }

            lock (list)
            {
                while (list.Count <= iteration)
                    list.Add(blockStateFactory.Create());
            }

            return list[iteration];
        }
    }
}
