using IS.Reading.Events;

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
        tester.Input("a");
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
        tester.Input(optionKey);
        await tester.ForwardAsync("thought* end", "person* leave: jane", "narration start", expected);
        await tester.ForwardEndAsync("narration end", "mc unset");

        await tester.BackwardAsync("mc: jane", "narration start", expected);
        await tester.BackwardAsync("narration end", "person* enter: jane", "thought* start", "thought*: Eu vou...");
        await tester.BackwardEndAsync("thought* end", "person* leave: jane", "mc unset");
    }

    const string conditionalStb =
@"' Storybasic 1.0

input entrada

@ jane
* Eu vou...
? selecao
  if entrada = '1'
    timelimit 5000
    default b
    a) Fazer silêncio
  else
    a) Sair rua afora
  end

  b)
    text Sair de perto
    disabled
    tip Disponível na próxima versão
  end

  c)
    if entrada = '1'
      text Perder a paciência
      icon pissed
    else
      text Bater o pé no chão
      icon angry
    end
  end
end

if selecao = 'a' 
  if entrada = '1'
    > Você fica em silêncio.
  else
    > Você sai andando rua afora.
  end
elseif selecao = 'b'
  > Você se afasta.
else
  if entrada = '1'
    > Você perde a paciência.
  else
    > Você bate o pé no chão com raiva.
  end
end";

    [Fact]
    public async Task Conditionals_1()
    {
        var input = "1";
        var tester = await StoryboardEventTester.CreateAsync(conditionalStb);

        await tester.ForwardAsync("input: entrada");
        tester.Input(input);
        await tester.ForwardAsync("person enter: jane", "thought start", "thought: Eu vou...");

        var e = tester.LastEvent<IBalloonTextEvent>();
        e.Choice.Should().BeEquivalentTo(new
        {
            TimeLimit = TimeSpan.FromSeconds(5),
            Default = "b"
        });

        var options = e.Choice.Options.GetEnumerator();
        
        options.MoveNext();
        options.Current.Should().BeEquivalentTo(new
        {
            Key = "a",
            Text = "Fazer silêncio",
            IsEnabled = true,
            ImageName = (string)null,
            Tip = (string)null
        });

        options.MoveNext();
        options.Current.Should().BeEquivalentTo(new
        {
            Key = "b",
            Text = "Sair de perto",
            IsEnabled = false,
            ImageName = (string)null,
            Tip = "Disponível na próxima versão"
        });

        options.MoveNext();
        options.Current.Should().BeEquivalentTo(new
        {
            Key = "c",
            Text = "Perder a paciência",
            IsEnabled = true,
            ImageName = "pissed",
            Tip = (string)null
        });
    }

    [Fact]
    public async Task Conditionals_2()
    {
        var input = "2";
        var tester = await StoryboardEventTester.CreateAsync(conditionalStb);

        await tester.ForwardAsync("input: entrada");
        tester.Input(input);
        await tester.ForwardAsync("person enter: jane", "thought start", "thought: Eu vou...");

        var e = tester.LastEvent<IBalloonTextEvent>();
        e.Choice.Should().BeEquivalentTo(new
        {
            TimeLimit = (TimeSpan?)null,
            Default = (string)null
        });

        var options = e.Choice.Options.GetEnumerator();

        options.MoveNext();
        options.Current.Should().BeEquivalentTo(new
        {
            Key = "a",
            Text = "Sair rua afora",
            IsEnabled = true,
            ImageName = (string)null,
            Tip = (string)null
        });

        options.MoveNext();
        options.Current.Should().BeEquivalentTo(new
        {
            Key = "b",
            Text = "Sair de perto",
            IsEnabled = false,
            ImageName = (string)null,
            Tip = "Disponível na próxima versão"
        });

        options.MoveNext();
        options.Current.Should().BeEquivalentTo(new
        {
            Key = "c",
            Text = "Bater o pé no chão",
            IsEnabled = true,
            ImageName = "angry",
            Tip = (string)null
        });
    }

    [Theory]
    [InlineData("1", "a", "Você fica em silêncio.")]
    [InlineData("1", "b", "Você se afasta.")]
    [InlineData("1", "c", "Você perde a paciência.")]
    [InlineData("2", "a", "Você sai andando rua afora.")]
    [InlineData("2", "b", "Você se afasta.")]
    [InlineData("2", "c", "Você bate o pé no chão com raiva.")]
    public async Task Conditionals_Results(string input, string choice, string narration)
    {
        var tester = await StoryboardEventTester.CreateAsync(conditionalStb);

        await tester.ForwardAsync("input: entrada");
        tester.Input(input);
        await tester.ForwardAsync("person enter: jane", "thought start", "thought: Eu vou...");
        tester.Input(choice);
        await tester.ForwardAsync("thought end", "person leave: jane", "narration start", $"narration: {narration}");
        await tester.ForwardEndAsync("narration end");

        await tester.BackwardAsync("narration start", $"narration: {narration}");
        await tester.BackwardAsync("narration end", "person enter: jane", "thought start", "thought: Eu vou...");
        await tester.BackwardAsync("thought end", "person leave: jane", "input: entrada");
        await tester.BackwardEndAsync();
    }
}
