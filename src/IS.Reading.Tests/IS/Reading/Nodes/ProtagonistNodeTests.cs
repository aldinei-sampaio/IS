using IS.Reading.Conditions;
using IS.Reading.Events;
using IS.Reading.Navigation;

namespace IS.Reading.Nodes;

public class ProtagonistNodeTests
{
    [Fact]
    public void Initialization()
    {
        var personName = "alice";
        var when = A.Dummy<ICondition>();
        var sut = new ProtagonistNode(personName, when);
        sut.ProtagonistName.Should().Be(personName);
        sut.When.Should().Be(when);
    }

    [Fact]
    public async Task OnEnterAsyncShouldReturnPreviousProtagonistName()
    {
        var context = A.Dummy<INavigationContext>();
        context.State.ProtagonistName = "rúcula";

        var sut = new ProtagonistNode("almeirão", null);

        var ret = await sut.EnterAsync(context);
        ret.Should().Be("rúcula");
    }

    [Theory]
    [InlineData(null, "omega")]
    [InlineData("omega", null)]
    [InlineData("alpha", "beta")]
    public async Task OnEnterAsyncShouldUpdateState(string currentValue, string newValue)
    {
        var context = A.Dummy<INavigationContext>();
        context.State.ProtagonistName = currentValue;

        var sut = new ProtagonistNode(newValue, null);
        await sut.EnterAsync(context);

        context.State.ProtagonistName.Should().Be(newValue);
    }

    [Theory]
    [InlineData(null, "omega")]
    [InlineData("omega", null)]
    [InlineData("alpha", "beta")]
    public async Task OnEnterShouldRaiseEvent(string currentValue, string newValue)
    {
        var context = A.Dummy<INavigationContext>();
        context.State.ProtagonistName = currentValue;

        var invoker = new TestInvoker(context);

        var sut = new ProtagonistNode(newValue, null);
        await sut.EnterAsync(context);

        invoker.ShouldContainSingle<IProtagonistChangeEvent>(
            i => i.Should().BeEquivalentTo(new { PersonName = newValue })
        );
    }

    [Theory]
    [InlineData(null)]
    [InlineData("omega")]
    public async Task OnEnterAsyncShouldNotRaiseEventIfProtagonistWasNotChanged(string value)
    {
        var context = A.Dummy<INavigationContext>();
        context.State.ProtagonistName = value;

        var invoker = new TestInvoker(context);

        var sut = new ProtagonistNode(value, null);
        var ret = await sut.EnterAsync(context);
        ret.Should().Be(value);

        invoker.Count.Should().Be(0);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("gama")]
    public async Task ShouldRaiseEventWithStateArg(string stageArg)
    {
        var context = A.Dummy<INavigationContext>();
        context.State.ProtagonistName = "alpha";

        var invoker = new TestInvoker(context);

        var sut = new ProtagonistNode("beta", null);
        await sut.EnterAsync(context, stageArg);

        invoker.ShouldContainSingle<IProtagonistChangeEvent>(
            i => i.Should().BeEquivalentTo(new { PersonName = stageArg })
        );

        context.State.ProtagonistName.Should().Be(stageArg);
    }
}
