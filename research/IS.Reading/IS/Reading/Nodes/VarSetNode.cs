using IS.Reading.Navigation;
using IS.Reading.Variables;

namespace IS.Reading.Nodes;

public class VarSetNode : INode
{
    public IVarSet VarSet { get; }

    public VarSetNode(IVarSet varSet)
        => VarSet = varSet;

    public Task<object?> EnterAsync(INavigationContext context)
        => Task.FromResult(VarSet.Execute(context.Variables));

    public Task EnterAsync(INavigationContext context, object? state)
    {
        if (state is not null && state is not string && state is not int)
            throw new ArgumentException($"Valor inválido: {state}", nameof(state));

        context.Variables[VarSet.Name] = state;
        return Task.CompletedTask;
    }
}
