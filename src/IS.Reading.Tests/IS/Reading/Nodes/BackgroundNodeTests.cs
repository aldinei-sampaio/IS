﻿using IS.Reading.Conditions;
using IS.Reading.Events;
using IS.Reading.Navigation;
using IS.Reading.State;

namespace IS.Reading.Nodes;

public class BackgroundNodeTests
{
    private readonly IBackgroundState initialState;
    private readonly IBackgroundState newState;
    private readonly ICondition when;
    private readonly INavigationContext context;
    private readonly BackgroundNode sut;

    public BackgroundNodeTests()
    {
        initialState = A.Dummy<IBackgroundState>();
        newState = A.Dummy<IBackgroundState>();
        when = A.Dummy<ICondition>();
        context = A.Dummy<INavigationContext>();
        context.State.Background = initialState;

        sut = new BackgroundNode(newState, when);
    }

    [Fact]
    public void ConstructorShouldInitializeStateAndWhen()
    {
        sut.State.Should().BeSameAs(newState);
        sut.When.Should().BeSameAs(when);
    }

    [Fact]
    public async Task EnterAsyncShouldRaiseEvent()
    {
        var invoker = new TestInvoker(context);
        await sut.EnterAsync(context);
        invoker.ShouldContainSingle<IBackgroundChangeEvent>(i => i.State.Should().BeSameAs(newState));
    }

    [Fact]
    public async Task EnterAsyncShouldChangeState()
    {
        await sut.EnterAsync(context);
        context.State.Background.Should().BeSameAs(newState);
    }

    [Fact]
    public async Task EnterAsyncShouldReturnPreviousState()
    {
        var ret = await sut.EnterAsync(context);
        ret.Should().BeSameAs(initialState);
    }

    [Fact]
    public async Task ShouldNotRaiseEventIfThereIsNoChangeInBackground()
    {
        var invoker = new TestInvoker(context);

        context.State.Background = newState;
        await sut.EnterAsync(context);

        invoker.Count.Should().Be(0);
    }

    [Fact]
    public async Task ShouldReturnPreviousStateEvenIfThereIsNoChangeInBackground()
    {
        context.State.Background = newState;
        var ret = await sut.EnterAsync(context);
        ret.Should().BeSameAs(newState);
    }

    [Fact]
    public async Task ShouldKeepStateIfThereIsNoChangeInBackground()
    {
        context.State.Background = newState;
        await sut.EnterAsync(context);
        context.State.Background.Should().BeSameAs(newState);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("Abc")]
    [InlineData(123)]
    public async Task ShouldDoNothingWhenStateArgIsNotBackgroundState(object stateArg)
    {
        var invoker = new TestInvoker(context);
        await sut.EnterAsync(context, stateArg);
        invoker.Count.Should().Be(0);
    }

    [Fact]
    public async Task ShouldRaiseEventWhenStateArgIsBackgroundState()
    {
        var invoker = new TestInvoker(context);
        await sut.EnterAsync(context, newState);
        invoker.ShouldContainSingle<IBackgroundChangeEvent>(i => i.State.Should().BeSameAs(newState));
    }
}
