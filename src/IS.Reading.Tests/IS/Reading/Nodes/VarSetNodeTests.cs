using IS.Reading.Navigation;
using IS.Reading.Variables;

namespace IS.Reading.Nodes;

public class VarSetNodeTests
{
    private readonly IVarSet varSet;
    private readonly VarSetNode sut;

    public VarSetNodeTests()
    {
        varSet = A.Fake<IVarSet>(i => i.Strict());
        sut = new VarSetNode(varSet);
    }

    [Fact]
    public void Initialization()
    {
        sut.VarSet.Should().BeSameAs(varSet);
    }

    [Fact]
    public async Task OnEnterAsyncShouldReturnReversedNode()
    {
        var context = A.Fake<INavigationContext>(i => i.Strict());
        var dic = A.Dummy<IVariableDictionary>();
        A.CallTo(() => context.Variables).Returns(dic);

        var reversedVarSet = A.Dummy<IVarSet>();
        A.CallTo(() => varSet.Execute(dic)).Returns(reversedVarSet);

        var reversedNode = await sut.EnterAsync(context);

        reversedNode.Should().BeOfType<VarSetNode>()
            .Which.VarSet.Should().BeSameAs(reversedVarSet);
    }
}
