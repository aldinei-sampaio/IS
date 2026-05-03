namespace IS.Reading.Navigation;

public class BackgroundEventTests
{
    [Fact]
    public async Task BackgroundWithArgument()
    {
        var stb =
@"' Storybasic 1.0
background fundo2
pause";

        var tester = await StoryboardEventTester.CreateAsync(stb);
        await tester.ForwardAsync("bg left: fundo2", "bg scroll");
        await tester.ForwardEndAsync("bg empty");
        await tester.BackwardAsync("bg right: fundo2");
        await tester.BackwardEndAsync("bg scroll", "bg empty");
    }

    [Fact]
    public async Task BackgroundWithArgument2()
    {
        var stb =
@"' Storybasic 1.0
background fundo1
pause
background fundo2
pause";

        var tester = await StoryboardEventTester.CreateAsync(stb);
        await tester.ForwardAsync("bg left: fundo1", "bg scroll");
        await tester.ForwardAsync("bg left: fundo2", "bg scroll");
        await tester.ForwardEndAsync("bg empty");
        await tester.BackwardAsync("bg right: fundo2");
        await tester.BackwardAsync("bg scroll", "bg right: fundo1");
        await tester.BackwardEndAsync("bg scroll", "bg empty");
    }

    [Fact]
    public async Task FowardAndBackward()
    {
        var stb =
@"' Storybasic 1.0
background
right fundo1
scroll
pause
color black
pause 250
color white
pause

background fundo2
pause";

        var tester = await StoryboardEventTester.CreateAsync(stb);

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