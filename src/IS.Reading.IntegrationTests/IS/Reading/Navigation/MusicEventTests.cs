namespace IS.Reading.Navigation;

public class MusicEventTests
{
    [Fact]
    public async Task Music()
    {
        var xml =
@"<storyboard>
    <music>open_sky</music>
    <pause />
    <music>never_look_back</music>
    <pause />
</storyboard>";

        var tester = await StoryboardEventTester.CreateAsync(xml);

        await tester.ForwardAsync("music: open_sky");
        await tester.ForwardAsync("music: never_look_back");
        await tester.ForwardEndAsync("music undefined");

        await tester.BackwardAsync("music: never_look_back");
        await tester.BackwardAsync("music: open_sky");
        await tester.BackwardEndAsync("music undefined");
    }
}
