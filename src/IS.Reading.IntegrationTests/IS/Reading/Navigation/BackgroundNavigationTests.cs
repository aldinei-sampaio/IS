using IS.Reading.Events;
using IS.Reading.Parsing;
using Microsoft.Extensions.DependencyInjection;

namespace IS.Reading.Navigation;

public class BackgroundNavigationTests
{
    private readonly IServiceProvider serviceProvider;

    public BackgroundNavigationTests()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddISReading();
        serviceProvider = serviceCollection.BuildServiceProvider();        
    }

    [Fact]
    public async Task FowardAndBackward()
    {
        var xml =
@"<storyboard>
    <background>
        <right>fundo1</right>
        <scroll />
        <pause />
        <color>black</color>
        <pause>250</pause>
        <color>white</color>
    </background>
    <pause />
    <background>fundo2</background>
    <pause />
</storyboard>";

        var eventHandler = new TestEventHandler();

        var parser = serviceProvider.GetRequiredService<IStoryboardParser>();
        using var sb = await parser.ParseAsync(new StringReader(xml));

        sb.Events.Subscribe(eventHandler.Handle);

        for (var n = 0; n <= 3; n++)
        {
            (await sb.MoveAsync(true)).Should().BeTrue();
            eventHandler.Check("bg right: fundo1", "bg scroll");

            (await sb.MoveAsync(true)).Should().BeTrue();
            eventHandler.Check("bg color: black", "pause: 250", "bg color: white");

            (await sb.MoveAsync(true)).Should().BeTrue();
            eventHandler.Check("bg left: fundo2", "bg scroll");

            (await sb.MoveAsync(true)).Should().BeFalse();
            eventHandler.Check("bg empty");

            (await sb.MoveAsync(false)).Should().BeTrue();
            eventHandler.Check("bg right: fundo2");

            (await sb.MoveAsync(false)).Should().BeTrue();
            eventHandler.Check("bg scroll", "bg color: white");

            (await sb.MoveAsync(false)).Should().BeTrue();
            eventHandler.Check("bg color: black", "pause: 250", "bg left: fundo1");

            (await sb.MoveAsync(false)).Should().BeFalse();
            eventHandler.Check("bg scroll", "bg empty");
        }
    }

    private class TestEventHandler
    {
        private readonly List<string> received = new();

        public Task Handle(IReadingEvent @event)
        { 
            received.Add(@event.ToString());
            return Task.CompletedTask;
        }
        
        public void Check(params string[] expectedEvents)
        {
            received.Should().BeEquivalentTo(expectedEvents);
            received.Clear();
        }
    }
}
