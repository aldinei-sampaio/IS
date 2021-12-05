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
    public async Task OnEnterAsyncShouldReturnReversedNode()
    {
        var context = A.Dummy<INavigationContext>();
        context.State.ProtagonistName = "rúcula";

        var sut = new ProtagonistNode("almeirão", null);

        var ret = await sut.EnterAsync(context);
        ret.Should().BeOfType<ProtagonistNode>()
            .Which.ProtagonistName.Should().Be("rúcula");
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

        var @event = invoker.Single<IProtagonistChangeEvent>();
        @event.PersonName.Should().Be(newValue);
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
        ret.Should().BeSameAs(sut);

        invoker.Count.Should().Be(0);
    }
}
