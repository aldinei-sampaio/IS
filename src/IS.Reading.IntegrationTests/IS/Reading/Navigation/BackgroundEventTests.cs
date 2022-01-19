namespace IS.Reading.Navigation;

public class BackgroundEventTests
{
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

        var tester = await StoryboardEventTester.CreateAsync(xml);

        for (var n = 0; n <= 3; n++)
        {
            await tester.ForwardAsync("bg right: fundo1", "bg scroll");
            await tester.ForwardAsync("bg color: black", "pause: 250", "bg color: white");
            await tester.ForwardAsync("bg left: fundo2", "bg scroll");
            await tester.ForwardEndAsync("bg empty");

            await tester.BackwardAsync("bg right: fundo2");
            await tester.BackwardAsync("bg scroll", "bg color: white");
            await tester.BackwardAsync("bg color: black", "bg left: fundo1");
            await tester.BackwardEndAsync("bg scroll", "bg empty");
        }
    }
}