using IS.Reading.Events;
using IS.Reading.Navigation;

namespace IS.Reading.Nodes;

public class PersonNodeTests
{
    [Fact]
    public void Initialization()
    {
        var childBlock = A.Dummy<IBlock>();
        var sut = new PersonNode("alpha", childBlock);
        sut.PersonName.Should().Be("alpha");
        sut.ChildBlock.Should().Be(childBlock);
    }

    [Theory]
    [InlineData("alpha", "beta")]
    [InlineData("alpha", "alpha")]
    [InlineData("beta", "beta")]
    public async Task OnEnterAsyncShouldRaiseEvent(string personName, string protagonist)
    {
        var isProtagonist = personName == protagonist;

        var context = A.Dummy<INavigationContext>();
        context.State.ProtagonistName = protagonist;

        var invoker = new TestInvoker(context);

        var childBlock = A.Dummy<IBlock>();
        var sut = new PersonNode(personName, childBlock);

        var ret = await sut.EnterAsync(context);
        ret.Should().BeSameAs(sut);

        var @event = invoker.Single<IPersonEnterEvent>();
        @event.PersonName.Should().Be(personName);
        @event.IsProtagonist.Should().Be(isProtagonist);
    }

    [Fact]
    public async Task OnEnterAsyncShouldUpdateState()
    {
        var context = A.Dummy<INavigationContext>();
        context.State.PersonName = null;

        var childBlock = A.Dummy<IBlock>();
        var sut = new PersonNode("amaterasu", childBlock);

        var ret = await sut.EnterAsync(context);
        ret.Should().BeSameAs(sut);

        context.State.PersonName.Should().Be("amaterasu");
    }

    [Theory]
    [InlineData("alpha", "beta")]
    [InlineData("alpha", "alpha")]
    [InlineData("beta", "beta")]
    public async Task OnLeaveAsyncShouldRaiseEvent(string personName, string protagonist)
    {
        var isProtagonist = personName == protagonist;

        var context = A.Dummy<INavigationContext>();
        context.State.ProtagonistName = protagonist;

        var invoker = new TestInvoker(context);

        var childBlock = A.Dummy<IBlock>();
        var sut = new PersonNode(personName, childBlock);

        await sut.LeaveAsync(context);

        var @event = invoker.Single<IPersonLeaveEvent>();
        @event.PersonName.Should().Be(personName);
        @event.IsProtagonist.Should().Be(isProtagonist);
    }

    [Fact]
    public async Task OnLeaveAsyncShouldUpdateState()
    {
        var context = A.Dummy<INavigationContext>();
        context.State.PersonName = "amaterasu";
        context.State.MoodType = MoodType.Happy;

        var childBlock = A.Dummy<IBlock>();
        var sut = new PersonNode("amaterasu", childBlock);

        await sut.LeaveAsync(context);

        context.State.PersonName.Should().BeNull();
        context.State.MoodType.Should().BeNull();
    }
}
