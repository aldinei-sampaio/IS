namespace IS.Reading.Navigation;

public class MusicEventTests
{
    [Fact]
    public async Task Music()
    {
        var stb =
@"' Storybasic 1.0
music open_sky
pause
music never_look_back
pause";

        var tester = await StoryboardEventTester.CreateAsync(stb);

        await tester.ForwardAsync("music: open_sky");
        await tester.ForwardAsync("music: never_look_back");
        await tester.ForwardEndAsync("music undefined");

        await tester.BackwardAsync("music: never_look_back");
        await tester.BackwardAsync("music: open_sky");
        await tester.BackwardEndAsync("music undefined");
    }
}
