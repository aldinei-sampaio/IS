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
        await tester.ForwardAsync("bg left: fundo2");
        await tester.ForwardEndAsync("bg empty");
        await tester.BackwardAsync("bg left: fundo2");
        await tester.BackwardEndAsync("bg empty");
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
        await tester.ForwardAsync("bg left: fundo1");
        await tester.ForwardAsync("bg left: fundo2");
        await tester.ForwardEndAsync("bg empty");
        await tester.BackwardAsync("bg left: fundo2");
        await tester.BackwardAsync("bg left: fundo1");
        await tester.BackwardEndAsync("bg empty");
    }

    [Theory]
    [InlineData("fadein",   "bg left: fundo2 [fadein]")]
    [InlineData("zoom",     "bg left: fundo2 [zoom]")]
    [InlineData("dissolve", "bg left: fundo2 [dissolve]")]
    [InlineData("flash",    "bg left: fundo2 [flash]")]
    public async Task BackgroundWithAnimation(string keyword, string expectedEvent)
    {
        var stb =
$@"' Storybasic 1.0
background fundo2 {keyword}
pause";

        var tester = await StoryboardEventTester.CreateAsync(stb);
        await tester.ForwardAsync(expectedEvent);
        await tester.ForwardEndAsync("bg empty");
        await tester.BackwardAsync("bg left: fundo2");
        await tester.BackwardEndAsync("bg empty");
    }

    [Theory]
    [InlineData("white",   "bg left: fundo2 [flash:white]")]
    [InlineData("red",     "bg left: fundo2 [flash:red]")]
    [InlineData("#ff0000", "bg left: fundo2 [flash:#ff0000]")]
    public async Task BackgroundWithFlashColor(string color, string expectedEvent)
    {
        var stb =
$@"' Storybasic 1.0
background fundo2 flash {color}
pause";

        var tester = await StoryboardEventTester.CreateAsync(stb);
        await tester.ForwardAsync(expectedEvent);
        await tester.ForwardEndAsync("bg empty");
        await tester.BackwardAsync("bg left: fundo2");
        await tester.BackwardEndAsync("bg empty");
    }

    [Theory]
    [InlineData("fadein",   "bg color: black [fadein]")]
    [InlineData("zoom",     "bg color: black [zoom]")]
    [InlineData("dissolve", "bg color: black [dissolve]")]
    [InlineData("flash",    "bg color: black [flash]")]
    public async Task ColorWithAnimation(string keyword, string expectedEvent)
    {
        var stb =
$@"' Storybasic 1.0
background
color black {keyword}
pause";

        var tester = await StoryboardEventTester.CreateAsync(stb);
        await tester.ForwardAsync(expectedEvent);
        await tester.ForwardEndAsync("bg empty");
        await tester.BackwardAsync("bg color: black");
        await tester.BackwardEndAsync("bg empty");
    }

    [Theory]
    [InlineData("white",   "bg color: black [flash:white]")]
    [InlineData("red",     "bg color: black [flash:red]")]
    public async Task ColorWithFlashColor(string flashColor, string expectedEvent)
    {
        var stb =
$@"' Storybasic 1.0
background
color black flash {flashColor}
pause";

        var tester = await StoryboardEventTester.CreateAsync(stb);
        await tester.ForwardAsync(expectedEvent);
        await tester.ForwardEndAsync("bg empty");
        await tester.BackwardAsync("bg color: black");
        await tester.BackwardEndAsync("bg empty");
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
        await tester.ForwardAsync("bg left: fundo2");
        await tester.ForwardEndAsync("bg empty");

        await tester.BackwardAsync("bg left: fundo2");
        await tester.BackwardAsync("bg color: white");
        await tester.BackwardAsync("bg color: black", "bg left: fundo1");
        await tester.BackwardEndAsync("bg scroll", "bg empty");
    }
}
