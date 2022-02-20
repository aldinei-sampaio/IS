namespace IS.Reading.Navigation;

public class BlockEventTests
{
    [Fact]
    public async Task If_FalseCondition()
    {
        var stb =
@"' Storybasic 1.0
tutorial
- texto1
- texto2
if a = 1
  narration
  - texto3
  - texto4
end
tutorial
- texto5";

        var tester = await StoryboardEventTester.CreateAsync(stb);

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
    public async Task If_TrueCondition()
    {
        var stb =
@"' Storybasic 1.0
tutorial
- texto1
- texto2
set a = 1
if a = 1
  narration
  - texto3
  - texto4
end
tutorial
- texto5";

        var tester = await StoryboardEventTester.CreateAsync(stb);

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
    public async Task IfElse_TrueCondition()
    {
        var stb =
@"' Storybasic 1.0
tutorial
- texto1
- texto2
set a = 1
if a = 1
  narration
  - texto3
  - texto4
else
  narration
  - texto4
  - texto5
end
tutorial
- texto5";

        var tester = await StoryboardEventTester.CreateAsync(stb);

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
    public async Task IfElse_FalseCondition()
    {
        var stb =
@"' Storybasic 1.0
tutorial
- texto1
- texto2
set a = 2
if a = 1
  narration
  - texto3
  - texto4
else
  narration
  - textoA
  - textoB
end
tutorial
- texto5";

        var tester = await StoryboardEventTester.CreateAsync(stb);

        await tester.ForwardAsync("tutorial start", "tutorial: texto1");
        await tester.ForwardAsync("tutorial: texto2");
        await tester.ForwardAsync("tutorial end", "narration start", "narration: textoA");
        await tester.ForwardAsync("narration: textoB");
        await tester.ForwardAsync("narration end", "tutorial start", "tutorial: texto5");
        await tester.ForwardEndAsync("tutorial end");

        await tester.BackwardAsync("tutorial start", "tutorial: texto5");
        await tester.BackwardAsync("tutorial end", "narration start", "narration: textoB");
        await tester.BackwardAsync("narration: textoA");
        await tester.BackwardAsync("narration end", "tutorial start", "tutorial: texto2");
        await tester.BackwardAsync("tutorial: texto1");
        await tester.BackwardEndAsync("tutorial end");
    }

    [Fact]
    public async Task While_FalseCondition()
    {
        var stb =
@"' Storybasic 1.0
tutorial
- texto1
while a = 1
  narration
  - texto2
end
tutorial
- texto3";

        var tester = await StoryboardEventTester.CreateAsync(stb);

        await tester.ForwardAsync("tutorial start", "tutorial: texto1");
        await tester.ForwardAsync("tutorial end", "tutorial start", "tutorial: texto3");
        await tester.ForwardEndAsync("tutorial end");

        await tester.BackwardAsync("tutorial start", "tutorial: texto3");
        await tester.BackwardAsync("tutorial end", "tutorial start", "tutorial: texto1");
        await tester.BackwardEndAsync("tutorial end");
    }

    [Fact]
    public async Task While_TrueCondition()
    {
        var stb =
@"' Storybasic 1.0
tutorial
- texto1
while a != 3
  narration
  - texto2
  set a++
end
tutorial
- texto3";

        var tester = await StoryboardEventTester.CreateAsync(stb);

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
    public async Task While_Concatenated()
    {
        var stb =
@"' Storybasic 1.0
tutorial
- texto1
while a != 2
  narration
  - texto2
  set a++
  while b != 2
    narration
    - texto3
    set b++
  end
end
tutorial
- texto4";

        var tester = await StoryboardEventTester.CreateAsync(stb);

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
