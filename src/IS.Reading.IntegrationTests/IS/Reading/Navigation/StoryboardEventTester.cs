using IS.Reading.Events;
using IS.Reading.Parsing;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace IS.Reading.Navigation;

public class StoryboardEventTester
{
    private readonly List<string> received = new();
    public IStoryboard Storyboard { get; }

    public static async Task<StoryboardEventTester> CreateAsync(string stb)
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddISReading();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var parser = serviceProvider.GetRequiredService<IStoryboardParser>();
        var storyboard = await parser.ParseAsync(new DocumentReader(new DocumentLineReader(new StringReader(stb))));
        return new(storyboard);
    }

    private StoryboardEventTester(IStoryboard storyboard)
    {
        storyboard.Events.Subscribe(Handle);
        this.Storyboard = storyboard;
    }

    private Task Handle(IReadingEvent @event)
    {
        received.Add(@event.ToString());
        return Task.CompletedTask;
    }

    public Task ForwardAsync(params string[] expectedEvents)
        => Check(received, Storyboard, true, false, expectedEvents);

    public Task ForwardEndAsync(params string[] expectedEvents)
        => Check(received, Storyboard, true, true, expectedEvents);

    public Task BackwardAsync(params string[] expectedEvents)
        => Check(received, Storyboard, false, false, expectedEvents);

    public Task BackwardEndAsync(params string[] expectedEvents)
        => Check(received, Storyboard, false, true, expectedEvents);

    public static async Task Check(List<string> received, IStoryboard storyboard, bool forward, bool atEnd, string[] expectedEvents)
    {
        received.Clear();
        (await storyboard.MoveAsync(forward)).Should().Be(!atEnd);
        received.Should().BeEquivalentTo(expectedEvents);
    }

    private async Task<bool> LogAsync(StringBuilder builder, bool forward)
    {
        received.Clear();
        var atEnd = !await Storyboard.MoveAsync(forward);
        if (atEnd)
        {
            if (forward)
                builder.Append("await tester.ForwardEndAsync(");
            else
                builder.Append("await tester.BackwardEndAsync(");
        }
        else
        {
            if (forward)
                builder.Append("await tester.ForwardAsync(");
            else
                builder.Append("await tester.BackwardAsync(");
        }
        for(var n = 0; n < received.Count; n++)
        {
            if (n > 0)
                builder.Append(", ");
            builder.Append('"');
            builder.Append(received[n]);
            builder.Append('"');
        }
        builder.Append(");");
        if (!atEnd)
            builder.AppendLine();
        return !atEnd;
    }

    public static async Task<string> GenerateLogAsync(string xml)
    {
        var builder = new StringBuilder();
        var tester = await CreateAsync(xml);
        while (await tester.LogAsync(builder, true))
        {
        }
        builder.AppendLine();
        builder.AppendLine();
        while (await tester.LogAsync(builder, false))
        {
        }
        return builder.ToString();
    }
}

