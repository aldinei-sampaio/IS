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

    [Fact]
    public async Task OnEnterAsyncShouldRaiseEvent()
    {
        var context = A.Dummy<INavigationContext>();
        context.State.ProtagonistName = null;

        var invoker = new TestInvoker(context);

        var sut = new ProtagonistNode("advocado", null);
        await sut.EnterAsync(context);

        var @event = invoker.Single<IProtagonistChangeEvent>();
        @event.PersonName.Should().Be("advocado");
    }

    [Fact]
    public async Task OnEnterAsyncShouldUpdateState()
    {
        var context = A.Dummy<INavigationContext>();
        context.State.ProtagonistName = "linguini";

        var sut = new ProtagonistNode("repasto", null);
        await sut.EnterAsync(context);

        context.State.ProtagonistName.Should().Be("repasto");
    }

    [Fact]
    public async Task OnEnterAsyncShouldNotRaiseEventIfProtagonistWasNotChanged()
    {
        var context = A.Dummy<INavigationContext>();
        context.State.ProtagonistName = "escandinavia";

        var invoker = new TestInvoker(context);

        var sut = new ProtagonistNode("escandinavia", null);
        var ret = await sut.EnterAsync(context);
        ret.Should().BeSameAs(sut);

        invoker.Count.Should().Be(0);
    }
}
