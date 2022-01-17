using IS.Reading.Navigation;
using IS.Reading.Variables;

namespace IS.Reading.Nodes;

public class VarSetNode : INode
{
    public IVarSet VarSet { get; }

    public VarSetNode(IVarSet varSet)
        => VarSet = varSet;

    private static Task<object?> Evaluate(INavigationContext context, IVarSet varSet)
        => Task.FromResult<object?>(varSet.Execute(context.Variables));

    public Task<object?> EnterAsync(INavigationContext context)
        => Evaluate(context, VarSet);

    public Task EnterAsync(INavigationContext context, object? state)
    {
        if (state is IVarSet varSet)
            Evaluate(context, varSet);
        return Task.CompletedTask;
    }
}
