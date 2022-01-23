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
    public async Task OnEnterAsyncShouldReturnReversedVarSet()
    {
        var context = A.Fake<INavigationContext>(i => i.Strict());
        var dic = A.Dummy<IVariableDictionary>();
        A.CallTo(() => context.Variables).Returns(dic);

        var reversedVarSet = A.Dummy<IVarSet>();
        A.CallTo(() => varSet.Execute(dic)).Returns(reversedVarSet);

        var reversedNode = await sut.EnterAsync(context);

        reversedNode.Should().BeSameAs(reversedVarSet);
        A.CallTo(() => varSet.Execute(dic)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("abc")]
    [InlineData(123)]
    public async Task ShouldSetValueToStateArg(object stageArg)
    {
        var context = A.Fake<INavigationContext>(i => i.Strict());
        var dic = A.Dummy<IVariableDictionary>();
        A.CallTo(() => context.Variables).Returns(dic);

        var varName = "Alpha";

        A.CallTo(() => varSet.Name).Returns(varName);
        A.CallToSet(() => context.Variables[varName]).To(stageArg).DoesNothing();

        await sut.EnterAsync(context, stageArg);

        A.CallToSet(() => context.Variables[varName]).To(stageArg).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(128.982)]
    public async Task InvalidValuesForStageArg(object value)
    {
        var context = A.Dummy<INavigationContext>();
        await Assert.ThrowsAsync<ArgumentException>(() => sut.EnterAsync(context, value));
    }
}
