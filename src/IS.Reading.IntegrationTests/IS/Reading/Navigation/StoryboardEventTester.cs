using IS.Reading.Events;
using IS.Reading.Parsing;
using Microsoft.Extensions.DependencyInjection;

namespace IS.Reading.Navigation;

public class StoryboardEventTester
{
    private readonly List<string> received = new();
    private readonly IStoryboard storyboard;

    public static async Task<StoryboardEventTester> CreateAsync(string xml)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddISReading();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var parser = serviceProvider.GetRequiredService<IStoryboardParser>();
        var storyboard = await parser.ParseAsync(new StringReader(xml));
        return new(storyboard);
    }

    private StoryboardEventTester(IStoryboard storyboard)
    {
        storyboard.Events.Subscribe(Handle);
        this.storyboard = storyboard;
    }

    private Task Handle(IReadingEvent @event)
    {
        received.Add(@event.ToString());
        return Task.CompletedTask;
    }

    public Task ForwardAsync(params string[] expectedEvents)
        => Check(received, storyboard, true, false, expectedEvents);

    public Task ForwardEndAsync(params string[] expectedEvents)
        => Check(received, storyboard, true, true, expectedEvents);

    public Task BackwardAsync(params string[] expectedEvents)
        => Check(received, storyboard, false, false, expectedEvents);

    public Task BackwardEndAsync(params string[] expectedEvents)
        => Check(received, storyboard, false, true, expectedEvents);

    public static async Task Check(List<string> received, IStoryboard storyboard, bool forward, bool atEnd, string[] expectedEvents)
    {
        received.Clear();
        (await storyboard.MoveAsync(forward)).Should().Be(!atEnd);
        received.Should().BeEquivalentTo(expectedEvents);
    }
}

