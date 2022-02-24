using System.Text;

namespace IS.Reading.Events;

public class EventManagerTests
{
    [Fact]
    public async Task NoHandlersShouldNotCauseError()
    {
        var sut = new EventManager();
        await sut.InvokeAsync(new TestEvent1());
    }

    [Fact]
    public async Task SubscribeAll()
    {
        var helper = new TestHelper();

        var sut = new EventManager();
        sut.Subscribe(i => helper.HandleAsync(i));

        await sut.InvokeAsync(new TestEvent1 { Name = "e1" });
        await sut.InvokeAsync(new TestEvent1 { Name = "e2" });
        await sut.InvokeAsync(new TestEvent2 { Name = "e3" });

        helper.Check("TestEvent1:e1 TestEvent1:e2 TestEvent2:e3 ");
    }

    [Fact]
    public async Task SingleHandler()
    {
        var helper = new TestHelper();

        var sut = new EventManager();
        sut.Subscribe<TestEvent1>(i => helper.HandleAsync(i));

        await sut.InvokeAsync(new TestEvent1 { Name = "e1" });

        helper.Check("TestEvent1:e1 ");
    }

    [Fact]
    public async Task Unhandled()
    {
        var helper = new TestHelper();

        var sut = new EventManager();
        sut.Subscribe<TestEvent1>(i => helper.HandleAsync(i));

        await sut.InvokeAsync(new TestEvent2 { Name = "e1" });

        helper.Check(string.Empty);
    }

    [Fact]
    public async Task TwoEventsOfSameType()
    {
        var helper = new TestHelper();

        var sut = new EventManager();
        sut.Subscribe<TestEvent1>(i => helper.HandleAsync(i));
        sut.Subscribe<TestEvent2>(i => helper.HandleAsync(i));

        await sut.InvokeAsync(new TestEvent1 { Name = "e1" });
        await sut.InvokeAsync(new TestEvent1 { Name = "e2" });
        await sut.InvokeAsync(new TestEvent2 { Name = "e3" });

        helper.Check("TestEvent1:e1 TestEvent1:e2 TestEvent2:e3 ");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public async Task MultipleSubscriptionsToSameEvent(int subscriptionCount)
    {
        var helper = new TestHelper();

        var sut = new EventManager();
        for (var n = 1; n <= subscriptionCount; n++)
            sut.Subscribe<TestEvent1>(i => helper.HandleAsync(i));

        await sut.InvokeAsync(new TestEvent1 { Name = "e1" });

        var expected = string.Concat(Enumerable.Repeat("TestEvent1:e1 ", subscriptionCount));
        helper.Check(expected);
    }

    [Fact]
    public async Task SubscriptToAll()
    {
        var helper = new TestHelper();

        var sut = new EventManager();
        sut.Subscribe(i => helper.HandleAsync(i));

        await sut.InvokeAsync(new TestEvent1 { Name = "e1" });
        await sut.InvokeAsync(new TestEvent1 { Name = "e2" });
        await sut.InvokeAsync(new TestEvent2 { Name = "e3" });

        helper.Check("TestEvent1:e1 TestEvent1:e2 TestEvent2:e3 ");
    }

    [Fact]
    public async Task BothTypesOfSubscriptions()
    {
        var helper = new TestHelper();

        var sut = new EventManager();
        sut.Subscribe(i => helper.HandleAsync(i));
        sut.Subscribe<TestEvent2>(i => helper.HandleAsync(i));

        await sut.InvokeAsync(new TestEvent1 { Name = "e1" });
        await sut.InvokeAsync(new TestEvent1 { Name = "e2" });
        await sut.InvokeAsync(new TestEvent2 { Name = "e3" });

        helper.Check("TestEvent1:e1 TestEvent1:e2 TestEvent2:e3 TestEvent2:e3 ");
    }

    [Fact]
    public async Task DisposeShouldUnsubscribeAll()
    {
        var helper = new TestHelper();

        var sut = new EventManager();
        sut.Subscribe(i => helper.HandleAsync(i));
        sut.Subscribe<TestEvent2>(i => helper.HandleAsync(i));

        sut.Dispose();
        sut.SubscriptionCount.Should().Be(0);

        await sut.InvokeAsync(new TestEvent1 { Name = "e1" });

        helper.Check("");
    }

    [Fact]
    public void SubscriptionCount()
    {
        var helper = new TestHelper();

        var sut = new EventManager();
        sut.SubscriptionCount.Should().Be(0);
        sut.Subscribe(i => helper.HandleAsync(i));
        sut.SubscriptionCount.Should().Be(1);
        sut.Subscribe<TestEvent1>(i => helper.HandleAsync(i));
        sut.SubscriptionCount.Should().Be(2);
        sut.Subscribe<TestEvent2>(i => helper.HandleAsync(i));
        sut.SubscriptionCount.Should().Be(3);

        sut.Dispose();
        sut.SubscriptionCount.Should().Be(0);
    }

    public class TestHelper
    {
        private readonly StringBuilder stringBuilder = new();

        public Task HandleAsync(IReadingEvent @event)
            => HandleAsync((TestEvent)@event);

        public Task HandleAsync(TestEvent @event)
        {
            stringBuilder.Append($"{@event.GetType().Name}:{@event.Name} "); 
            return Task.CompletedTask;
        }

        public void Check(string expected)
        {
            var actual = stringBuilder.ToString();
            actual.Should().Be(expected);
        }
    }

    public class TestEvent : IReadingEvent
    {
        public string Name { get; init; }
    }

    public class TestEvent1 : TestEvent
    {
    }

    public class TestEvent2 : TestEvent
    {
    }
}
