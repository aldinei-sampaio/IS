namespace IS.Reading.Navigation;

public class BlockEventTests
{
    [Fact]
    public async Task DoWhen_FalseCondition()
    {
        var xml =
@"<storyboard>
    <tutorial>texto1</tutorial>
    <tutorial>texto2</tutorial>
    <do when=""a=1"">
        <narration>texto3</narration>
        <narration>texto4</narration>
    </do>
    <tutorial>texto5</tutorial>
</storyboard>";

        var tester = await StoryboardEventTester.CreateAsync(xml);

        await tester.ForwardAsync("tutorial start", "tutorial: texto1");
        await tester.ForwardAsync("tutorial: texto2");
        await tester.ForwardAsync("tutorial end", "tutorial start", "tutorial: texto5");
        await tester.ForwardEndAsync("tutorial end");

        await tester.BackwardAsync("tutorial start", "tutorial: texto5");
        await tester.BackwardAsync("tutorial end", "tutorial start", "tutorial: texto2");
        await tester.BackwardAsync("tutorial: texto1");
        await tester.BackwardEndAsync("tutorial end");
    }

    [Fact]
    public async Task TutorialAndNarration()
    {
        var xml =
@"<storyboard>
    <tutorial>texto1</tutorial>
    <tutorial>texto2</tutorial>
    <set>a=1</set>
    <do when=""a=1"">
        <narration>texto3</narration>
        <narration>texto4</narration>
    </do>
    <tutorial>texto5</tutorial>
</storyboard>";

        var tester = await StoryboardEventTester.CreateAsync(xml);

        await tester.ForwardAsync("tutorial start", "tutorial: texto1");
        await tester.ForwardAsync("tutorial: texto2");
        await tester.ForwardAsync("tutorial end", "narration start", "narration: texto3");
        await tester.ForwardAsync("narration: texto4");
        await tester.ForwardAsync("narration end", "tutorial start", "tutorial: texto5");
        await tester.ForwardEndAsync("tutorial end");

        await tester.BackwardAsync("tutorial start", "tutorial: texto5");
        await tester.BackwardAsync("tutorial end", "narration start", "narration: texto4");
        await tester.BackwardAsync("narration: texto3");
        await tester.BackwardAsync("narration end", "tutorial start", "tutorial: texto2");
        await tester.BackwardAsync("tutorial: texto1");
        await tester.BackwardEndAsync("tutorial end");
    }

    [Fact]
    public async Task DoWhile_FalseCondition()
    {
        var xml =
@"<storyboard>
    <tutorial>texto1</tutorial>
    <do while=""a=1"">
        <narration>texto2</narration>
    </do>
    <tutorial>texto3</tutorial>
</storyboard>";

        var tester = await StoryboardEventTester.CreateAsync(xml);

        await tester.ForwardAsync("tutorial start", "tutorial: texto1");
        await tester.ForwardAsync("tutorial end", "tutorial start", "tutorial: texto3");
        await tester.ForwardEndAsync("tutorial end");

        await tester.BackwardAsync("tutorial start", "tutorial: texto3");
        await tester.BackwardAsync("tutorial end", "tutorial start", "tutorial: texto1");
        await tester.BackwardEndAsync("tutorial end");
    }

    [Fact]
    public async Task DoWhile()
    {
        var xml =
@"<storyboard>
    <tutorial>texto1</tutorial>
    <do while=""a!=3"">
        <narration>texto2</narration>
        <set>a++</set>
    </do>
    <tutorial>texto3</tutorial>
</storyboard>";

        var tester = await StoryboardEventTester.CreateAsync(xml);

        await tester.ForwardAsync("tutorial start", "tutorial: texto1");
        await tester.ForwardAsync("tutorial end", "narration start", "narration: texto2");
        await tester.ForwardAsync("narration end", "narration start", "narration: texto2");
        await tester.ForwardAsync("narration end", "narration start", "narration: texto2");
        await tester.ForwardAsync("narration end", "tutorial start", "tutorial: texto3");
        await tester.ForwardEndAsync("tutorial end");

        await tester.BackwardAsync("tutorial start", "tutorial: texto3");
        await tester.BackwardAsync("tutorial end", "narration start", "narration: texto2");
        await tester.BackwardAsync("narration end", "narration start", "narration: texto2");
        await tester.BackwardAsync("narration end", "narration start", "narration: texto2");
        await tester.BackwardAsync("narration end", "tutorial start", "tutorial: texto1");
        await tester.BackwardEndAsync("tutorial end");
    }

    [Fact]
    public async Task DoWhile_Concatenated()
    {
        var xml =
@"<storyboard>
    <tutorial>texto1</tutorial>
    <do while=""a!=2"">
        <narration>texto2</narration>
        <set>a++</set>
        <do while=""b!=2"">
            <narration>texto3</narration>
            <set>b++</set>
        </do>
    </do>
    <tutorial>texto4</tutorial>
</storyboard>";

        var tester = await StoryboardEventTester.CreateAsync(xml);

        await tester.ForwardAsync("tutorial start", "tutorial: texto1");
        await tester.ForwardAsync("tutorial end", "narration start", "narration: texto2");
        await tester.ForwardAsync("narration end", "narration start", "narration: texto3");
        await tester.ForwardAsync("narration end", "narration start", "narration: texto3");
        await tester.ForwardAsync("narration end", "narration start", "narration: texto2");
        await tester.ForwardAsync("narration end", "tutorial start", "tutorial: texto4");
        await tester.ForwardEndAsync("tutorial end");

        await tester.BackwardAsync("tutorial start", "tutorial: texto4");
        await tester.BackwardAsync("tutorial end", "narration start", "narration: texto2");
        await tester.BackwardAsync("narration end", "narration start", "narration: texto3");
        await tester.BackwardAsync("narration end", "narration start", "narration: texto3");
        await tester.BackwardAsync("narration end", "narration start", "narration: texto2");
        await tester.BackwardAsync("narration end", "tutorial start", "tutorial: texto1");
        await tester.BackwardEndAsync("tutorial end");
    }
}
