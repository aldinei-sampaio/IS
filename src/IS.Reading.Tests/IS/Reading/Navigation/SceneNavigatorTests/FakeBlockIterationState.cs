using IS.Reading.State;

namespace IS.Reading.Navigation.SceneNavigatorTests;

public class FakeBlockIterationState : IBlockIterationState
{
    public Stack<object> BackwardStack { get; } = new();
    public int? CurrentNodeIndex { get; set; }
    public INode CurrentNode { get; set; }

    public IBlockStateDictionary Children { get; } = new FakeBlockStateDictionary();
}
