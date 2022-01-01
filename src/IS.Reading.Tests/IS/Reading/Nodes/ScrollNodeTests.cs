﻿using IS.Reading.Conditions;
using IS.Reading.Events;
using IS.Reading.Navigation;
using IS.Reading.State;

namespace IS.Reading.Nodes;

public class ScrollNodeTests
{
    [Fact]
    public void Initialization()
    {
        var when = A.Dummy<ICondition>();

        var sut = new ScrollNode(when);
        sut.When.Should().BeSameAs(when);
    }

    [Theory]
    [InlineData(BackgroundPosition.Left, BackgroundPosition.Right)]
    [InlineData(BackgroundPosition.Right, BackgroundPosition.Left)]
    public async Task EnterAsync(BackgroundPosition oldPosition, BackgroundPosition newPosition)
    {
        var oldState = new BackgroundState("alpha", BackgroundType.Image, oldPosition);
        var newState = new BackgroundState("alpha", BackgroundType.Image, newPosition);

        var when = A.Dummy<ICondition>();

        var context = A.Dummy<INavigationContext>();
        context.State.Background = oldState;
        var invoker = new TestInvoker(context);

        var sut = new ScrollNode(when);
        var ret = await sut.EnterAsync(context);

        invoker.ShouldContainSingle<IBackgroundScrollEvent>(
            i => i.Should().BeEquivalentTo(new { Position = newPosition })
        );

        context.State.Background.Should().Be(newState);

        ret.Should().BeOfType<ScrollNode>()
            .Which.Should().BeEquivalentTo(new { When = when });
    }

    [Theory]
    [InlineData(BackgroundType.Image, BackgroundPosition.Undefined)]
    [InlineData(BackgroundType.Undefined, BackgroundPosition.Left)]
    [InlineData(BackgroundType.Undefined, BackgroundPosition.Right)]
    [InlineData(BackgroundType.Undefined, BackgroundPosition.Undefined)]
    [InlineData(BackgroundType.Color, BackgroundPosition.Left)]
    [InlineData(BackgroundType.Color, BackgroundPosition.Right)]
    [InlineData(BackgroundType.Color, BackgroundPosition.Undefined)]
    public async Task IgnoreOnInvalidState(BackgroundType type, BackgroundPosition position)
    {
        var oldState = new BackgroundState("alpha", type, position);
        var context = A.Dummy<INavigationContext>();
        context.State.Background = oldState;
        var invoker = new TestInvoker(context);

        var sut = new ScrollNode(null);
        var ret = await sut.EnterAsync(context);

        ret.Should().BeSameAs(sut);
        context.State.Background.Should().BeSameAs(oldState);
        invoker.Count.Should().Be(0);
    }
}
