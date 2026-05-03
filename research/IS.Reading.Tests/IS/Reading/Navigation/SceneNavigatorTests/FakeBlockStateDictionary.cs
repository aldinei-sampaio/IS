using IS.Reading.State;

namespace IS.Reading.Navigation.SceneNavigatorTests;

public class FakeBlockStateDictionary : IBlockStateDictionary
{
    private static readonly Dictionary<int, IBlockState> dic = new();
    public IBlockState this[int blockId]
    {
        get
        {
            if (dic.TryGetValue(blockId, out var state))
                return state;
            state = new FakeBlockState();
            dic[blockId] = state;
            return state;
        }
    }
}
