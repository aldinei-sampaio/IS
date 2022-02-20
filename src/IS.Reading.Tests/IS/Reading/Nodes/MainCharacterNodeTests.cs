using IS.Reading.Conditions;
using IS.Reading.Events;
using IS.Reading.Navigation;

namespace IS.Reading.Nodes;

public class MainCharacterNodeTests
{
    [Fact]
    public void Initialization()
    {
        var personName = "alice";
        var when = A.Dummy<ICondition>();
        var sut = new MainCharacterNode(personName);
        sut.MainCharacterName.Should().Be(personName);
    }

    [Fact]
    public async Task OnEnterAsyncShouldReturnPreviousMainCharacterName()
    {
        var context = A.Dummy<INavigationContext>();
        context.State.MainCharacterName = "rúcula";

        var sut = new MainCharacterNode("almeirão");

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
        context.State.MainCharacterName = currentValue;

        var sut = new MainCharacterNode(newValue);
        await sut.EnterAsync(context);

        context.State.MainCharacterName.Should().Be(newValue);
    }

    [Theory]
    [InlineData(null, "omega")]
    [InlineData("omega", null)]
    [InlineData("alpha", "beta")]
    public async Task OnEnterShouldRaiseEvent(string currentValue, string newValue)
    {
        var context = A.Dummy<INavigationContext>();
        context.State.MainCharacterName = currentValue;

        var invoker = new TestInvoker(context);

        var sut = new MainCharacterNode(newValue);
        await sut.EnterAsync(context);

        invoker.ShouldContainSingle<IMainCharacterChangeEvent>(
            i => i.Should().BeEquivalentTo(new { PersonName = newValue })
        );
    }

    [Theory]
    [InlineData(null)]
    [InlineData("omega")]
    public async Task OnEnterAsyncShouldNotRaiseEventIfMainCharacterWasNotChanged(string value)
    {
        var context = A.Dummy<INavigationContext>();
        context.State.MainCharacterName = value;

        var invoker = new TestInvoker(context);

        var sut = new MainCharacterNode(value);
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
        context.State.MainCharacterName = "alpha";

        var invoker = new TestInvoker(context);

        var sut = new MainCharacterNode("beta");
        await sut.EnterAsync(context, stageArg);

        invoker.ShouldContainSingle<IMainCharacterChangeEvent>(
            i => i.Should().BeEquivalentTo(new { PersonName = stageArg })
        );

        context.State.MainCharacterName.Should().Be(stageArg);
    }
}
