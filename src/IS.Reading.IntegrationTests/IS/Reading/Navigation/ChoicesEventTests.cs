namespace IS.Reading.Navigation;

public class ChoicesEventTests
{
    [Fact]
    public async Task SimpleChoice()
    {
        var stb =
@"' Storybasic 1.0
mc jane
@ jane
* Eu vou...
? selecao
  a) Fazer silêncio
  b) Sair de perto
end";

        var tester = await StoryboardEventTester.CreateAsync(stb);

        await tester.ForwardAsync("mc: jane", "person* enter: jane", "thought* start", "thought*: Eu vou...");
        tester.SetChoice("a");
        await tester.ForwardEndAsync("thought* end", "person* leave: jane", "mc unset");

        await tester.BackwardAsync("mc: jane", "person* enter: jane", "thought* start", "thought*: Eu vou...");
        await tester.BackwardEndAsync("thought* end", "person* leave: jane", "mc unset");
    }

    [Theory]
    [InlineData("a", "narration: Você fica em silêncio.")]
    [InlineData("b", "narration: Você se afasta.")]
    public async Task SimpleChoiceSelection(string optionKey, string expected)
    {
        var stb =
@"' Storybasic 1.0
mc jane
@ jane
* Eu vou...
? selecao
  a) Fazer silêncio
  b) Sair de perto
end

if selecao = 'a'
  > Você fica em silêncio.
else
  > Você se afasta.
end";

        var tester = await StoryboardEventTester.CreateAsync(stb);

        await tester.ForwardAsync("mc: jane", "person* enter: jane", "thought* start", "thought*: Eu vou...");
        tester.SetChoice(optionKey);
        await tester.ForwardAsync("thought* end", "person* leave: jane", "narration start", expected);
        await tester.ForwardEndAsync("narration end", "mc unset");

        await tester.BackwardAsync("mc: jane", "narration start", expected);
        await tester.BackwardAsync("narration end", "person* enter: jane", "thought* start", "thought*: Eu vou...");
        await tester.BackwardEndAsync("thought* end", "person* leave: jane", "mc unset");
    }
}
