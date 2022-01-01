using IS.Reading.Navigation;
using IS.Reading.Variables;

namespace IS.Reading.Nodes;

public class VarSetNode : INode
{
    public IVarSet VarSet { get; }

    public VarSetNode(IVarSet varSet)
        => VarSet = varSet;

    public Task<INode> EnterAsync(INavigationContext context)
    {
        var reversed = VarSet.Execute(context.Variables);
        return Task.FromResult<INode>(new VarSetNode(reversed));
    }
}
