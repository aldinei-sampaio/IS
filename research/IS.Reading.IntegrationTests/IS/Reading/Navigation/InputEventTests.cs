using IS.Reading.Events;

namespace IS.Reading.Navigation;

public class InputEventTests
{
    [Theory]
    [InlineData("Maria")]
    [InlineData("Pedro")]
    public async Task Input(string name)
    {
        var stb =
@"' Storybasic 1.0
set mc = 'Belizar'

input mc
title Nomear Personagem
text Qual é o seu nome?
len 14
conf Seu nome é {{0}}?

> Saudações, {mc}!";

        var forwardEvent = new
        {
            Key = "mc",
            Title = "Nomear Personagem",
            Text = "Qual é o seu nome?",
            MaxLength = 14,
            Confirmation = "Seu nome é {0}?",
            DefaultValue = "Belizar"
        };

        var backwardEvent = new
        {
            forwardEvent.Key,
            forwardEvent.Title,
            forwardEvent.Text,
            forwardEvent.MaxLength,
            forwardEvent.Confirmation,
            DefaultValue = name
        };

        var tester = await StoryboardEventTester.CreateAsync(stb);

        await tester.ForwardAsync("input: mc");
        tester.LastEvent<IInputEvent>().Should().BeEquivalentTo(forwardEvent);
        tester.Input(name);
        await tester.ForwardAsync("narration start", $"narration: Saudações, {name}!");
        await tester.ForwardEndAsync("narration end");

        await tester.BackwardAsync("narration start", $"narration: Saudações, {name}!");
        await tester.BackwardAsync("narration end", "input: mc");
        tester.LastEvent<IInputEvent>().Should().BeEquivalentTo(backwardEvent);
        await tester.BackwardEndAsync();

        await tester.ForwardAsync("input: mc");
        tester.LastEvent<IInputEvent>().Should().BeEquivalentTo(forwardEvent);
        tester.Input(name);
        await tester.ForwardAsync("narration start", $"narration: Saudações, {name}!");
        await tester.ForwardEndAsync("narration end");
    }
}
