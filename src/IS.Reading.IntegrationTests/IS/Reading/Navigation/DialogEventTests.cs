namespace IS.Reading.Navigation;

public class DialogEventTests
{
    [Fact]
    public async Task TutorialAndNarration()
    {
        var xml =
@"<storyboard>
    <tutorial>texto1</tutorial>
    <tutorial>texto2</tutorial>
    <narration>texto3</narration>
    <narration>texto4</narration>
    <tutorial>texto5</tutorial>
    <narration>texto6</narration>
</storyboard>";

        var tester = await StoryboardEventTester.CreateAsync(xml);

        //for (var n = 0; n <= 3; n++)
        //{
            await tester.ForwardAsync("tutorial start", "tutorial: texto1");
            await tester.ForwardAsync("tutorial: texto2");
            await tester.ForwardAsync("tutorial end", "narration start", "narration: texto3");
            await tester.ForwardAsync("narration: texto4");
            await tester.ForwardAsync("narration end", "tutorial start", "tutorial: texto5");
            await tester.ForwardAsync("tutorial end", "narration start", "narration: texto6");
            await tester.ForwardEndAsync("narration end");

        //    await tester.BackwardAsync("bg right: fundo2");
        //    await tester.BackwardAsync("bg scroll", "bg color: white");
        //    await tester.BackwardAsync("bg color: black", "pause: 250", "bg left: fundo1");
        //    await tester.BackwardEndAsync("bg scroll", "bg empty");
        //}
    }
}
