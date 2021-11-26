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
    public async Task Test()
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

        var parser = serviceProvider.GetRequiredService<IStoryboardParser>();
        using var sb = await parser.ParseAsync(new StringReader(xml));

        //sb.Events.Subscribe<BackgroundChangeEvent>(i => DoSomethingAsync(i));

        await sb.MoveAsync(true);
    }
}
