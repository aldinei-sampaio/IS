using IS.Reading.Conditions;
using IS.Reading.Events;
using IS.Reading.Navigation;

namespace IS.Reading.Nodes;

public class MusicNodeTests
{
    [Fact]
    public void Initialization()
    {
        var musicName = "never_look_back";
        var when = A.Dummy<ICondition>();
        var sut = new MusicNode(musicName, when);
        sut.MusicName.Should().Be(musicName);
        sut.When.Should().Be(when);
    }

    [Fact]
    public async Task OnEnterAsyncShouldReturnLastMusicName()
    {
        var context = A.Dummy<INavigationContext>();
        context.State.MusicName = "enigma";

        var sut = new MusicNode("free_mind", null);

        var ret = await sut.EnterAsync(context);
        ret.Should().Be("enigma");
    }

    [Fact]
    public async Task OnEnterAsyncShouldRaiseEvent()
    {
        var context = A.Dummy<INavigationContext>();
        context.State.MusicName = null;

        var invoker = new TestInvoker(context);

        var sut = new MusicNode("goodbye", null);
        await sut.EnterAsync(context);

        invoker.ShouldContainSingle<IMusicChangeEvent>(
            i => i.Should().BeEquivalentTo(new { MusicName = "goodbye" })
        );
    }

    [Fact]
    public async Task OnEnterAsyncShouldUpdateState()
    {
        var context = A.Dummy<INavigationContext>();
        context.State.MusicName = "sad_story";

        var sut = new MusicNode("open_sky", null);
        await sut.EnterAsync(context);

        context.State.MusicName.Should().Be("open_sky");
    }

    [Fact]
    public async Task OnEnterAsyncShouldNotRaiseEventIfMusicWasNotChanged()
    {
        var context = A.Dummy<INavigationContext>();
        context.State.MusicName = "crime_fighter";

        var invoker = new TestInvoker(context);

        var sut = new MusicNode("crime_fighter", null);
        var ret = await sut.EnterAsync(context);
        ret.Should().Be("crime_fighter");

        invoker.Count.Should().Be(0);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("helloworld")]
    public async Task ShouldRaiseEventWithStateArg(string stateArg)
    {
        var context = A.Dummy<INavigationContext>();
        context.State.MusicName = "theme";

        var invoker = new TestInvoker(context);

        var sut = new MusicNode("goodbye", null);
        await sut.EnterAsync(context, stateArg);

        invoker.ShouldContainSingle<IMusicChangeEvent>(
            i => i.Should().BeEquivalentTo(new { MusicName = stateArg })
        );
    }
}
