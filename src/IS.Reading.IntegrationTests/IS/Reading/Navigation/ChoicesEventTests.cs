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
  a Fazer silêncio
  b Sair de perto
end";

        var tester = await StoryboardEventTester.CreateAsync(stb);

        await tester.ForwardAsync("mc: jane", "person* enter: jane", "thought* start", "thought*: Eu vou...");
        await tester.ForwardEndAsync("thought* end", "person* leave: jane", "mc unset");

        await tester.BackwardAsync("mc: jane", "person* enter: jane", "thought* start", "thought*: Eu vou...");
        await tester.BackwardEndAsync("thought* end", "person* leave: jane", "mc unset");
    }
}
