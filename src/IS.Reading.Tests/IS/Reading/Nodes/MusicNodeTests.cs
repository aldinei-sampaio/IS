﻿using IS.Reading.Conditions;
using IS.Reading.Events;
using IS.Reading.Navigation;

namespace IS.Reading.Nodes;
public class MusicNodeTests
{
    [Fact]
    public void Initialization()
    {
        var musicName = "never_look_back";
        var sut = new MusicNode(musicName);
        sut.MusicName.Should().Be(musicName);
    }

    [Fact]
    public async Task OnEnterAsyncShouldReturnLastMusicName()
    {
        var context = A.Dummy<INavigationContext>();
        context.State.MusicName = "enigma";

        var sut = new MusicNode("free_mind");

        var ret = await sut.EnterAsync(context);
        ret.Should().Be("enigma");
    }

    [Fact]
    public async Task OnEnterAsyncShouldRaiseEvent()
    {
        var context = A.Dummy<INavigationContext>();
        context.State.MusicName = null;

        var invoker = new TestInvoker(context);

        var sut = new MusicNode("goodbye");
        await sut.EnterAsync(context);

        invoker.ShouldHadReceived<IMusicChangeEvent>(
            i => i.Should().BeEquivalentTo(new { MusicName = "goodbye" })
        );
    }

    [Fact]
    public async Task OnEnterAsyncShouldUpdateState()
    {
        var context = A.Dummy<INavigationContext>();
        context.State.MusicName = "sad_story";

        var sut = new MusicNode("open_sky");
        await sut.EnterAsync(context);

        context.State.MusicName.Should().Be("open_sky");
    }

    [Fact]
    public async Task OnEnterAsyncShouldNotRaiseEventIfMusicWasNotChanged()
    {
        var context = A.Dummy<INavigationContext>();
        context.State.MusicName = "crime_fighter";

        var invoker = new TestInvoker(context);

        var sut = new MusicNode("crime_fighter");
        var ret = await sut.EnterAsync(context);
        ret.Should().Be("crime_fighter");

        invoker.HadReceivedEvent.Should().BeFalse();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("helloworld")]
    public async Task ShouldRaiseEventWithStateArg(string stateArg)
    {
        var context = A.Dummy<INavigationContext>();
        context.State.MusicName = "theme";

        var invoker = new TestInvoker(context);

        var sut = new MusicNode("goodbye");
        await sut.EnterAsync(context, stateArg);

        invoker.ShouldHadReceived<IMusicChangeEvent>(
            i => i.Should().BeEquivalentTo(new { MusicName = stateArg })
        );
    }
}
