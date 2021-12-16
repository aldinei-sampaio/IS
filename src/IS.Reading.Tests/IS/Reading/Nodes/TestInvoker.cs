using FluentAssertions.Execution;
using IS.Reading.Events;
using IS.Reading.Navigation;

namespace IS.Reading.Nodes;

public class TestInvoker : IEventInvoker
{
    public TestInvoker(INavigationContext context)
    {
        A.CallTo(() => context.Events).Returns(this);
    }

    private readonly List<IReadingEvent> invoked = new();
    
    private readonly List<Type> invokedTypes = new();

    public Task InvokeAsync<T>(T @event) where T : IReadingEvent
    {
        invoked.Add(@event);
        invokedTypes.Add(typeof(T));
        return Task.CompletedTask;
    }

    public void ShouldContainSingle<T>(Action<T> validation) where T : IReadingEvent
    {
        using (new AssertionScope())
            validation.Invoke(Single<T>());
    }

    private T Single<T>() where T : IReadingEvent
    {
        invoked.Should().HaveCount(1);
        invokedTypes[0].Should().BeSameAs(typeof(T));
        return (T)invoked[0];
    }

    public int Count => invoked.Count;

    public void Clear()
    {
        invoked.Clear();
        invokedTypes.Clear();
    }
}
