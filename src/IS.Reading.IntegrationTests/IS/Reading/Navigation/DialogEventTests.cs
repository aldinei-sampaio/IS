﻿namespace IS.Reading.Navigation;

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

        await tester.ForwardAsync("tutorial start", "tutorial: texto1");
        await tester.ForwardAsync("tutorial: texto2");
        await tester.ForwardAsync("tutorial end", "narration start", "narration: texto3");
        await tester.ForwardAsync("narration: texto4");
        await tester.ForwardAsync("narration end", "tutorial start", "tutorial: texto5");
        await tester.ForwardAsync("tutorial end", "narration start", "narration: texto6");
        await tester.ForwardEndAsync("narration end");

        await tester.BackwardAsync("narration start", "narration: texto6");
        await tester.BackwardAsync("narration end", "tutorial start", "tutorial: texto5");
        await tester.BackwardAsync("tutorial end", "narration start", "narration: texto4");
        await tester.BackwardAsync("narration: texto3");
        await tester.BackwardAsync("narration end", "tutorial start", "tutorial: texto2");
        await tester.BackwardAsync("tutorial: texto1");
        await tester.BackwardEndAsync("tutorial end");
    }

    [Fact]
    public async Task ProtagonistSpeechAndThought()
    {
        var xml =
@"<storyboard>
    <protagonist>joara</protagonist>
    <person>joara</person>
    <speech>texto1</speech>
    <speech>texto2</speech>
    <thought>texto3</thought>
    <speech>texto4</speech>
    <thought>texto5</thought>
    <thought>texto6</thought>
</storyboard>";

        var tester = await StoryboardEventTester.CreateAsync(xml);

        await tester.ForwardAsync("protagonist: joara", "person* enter: joara", "speech* start", "speech*: texto1");
        await tester.ForwardAsync("speech*: texto2");
        await tester.ForwardAsync("speech* end", "thought* start", "thought*: texto3");
        await tester.ForwardAsync("thought* end", "speech* start", "speech*: texto4");
        await tester.ForwardAsync("speech* end", "thought* start", "thought*: texto5");
        await tester.ForwardAsync("thought*: texto6");
        await tester.ForwardEndAsync("thought* end", "person* leave: joara", "protagonist undefined");

        await tester.BackwardAsync("protagonist: joara", "person* enter: joara", "thought* start", "thought*: texto6");
        await tester.BackwardAsync("thought*: texto5");
        await tester.BackwardAsync("thought* end", "speech* start", "speech*: texto4");
        await tester.BackwardAsync("speech* end", "thought* start", "thought*: texto3");
        await tester.BackwardAsync("thought* end", "speech* start", "speech*: texto2");
        await tester.BackwardAsync("speech*: texto1");
        await tester.BackwardEndAsync("speech* end", "person* leave: joara", "protagonist undefined");
    }

    [Fact]
    public async Task InterlocutorSpeechAndThought()
    {
        var xml =
@"<storyboard>
    <person>clodoaldo</person>
    <speech>texto1</speech>
    <speech>texto2</speech>
    <thought>texto3</thought>
    <speech>texto4</speech>
    <thought>texto5</thought>
    <thought>texto6</thought>
</storyboard>";

        var tester = await StoryboardEventTester.CreateAsync(xml);

        await tester.ForwardAsync("person enter: clodoaldo", "speech start", "speech: texto1");
        await tester.ForwardAsync("speech: texto2");
        await tester.ForwardAsync("speech end", "thought start", "thought: texto3");
        await tester.ForwardAsync("thought end", "speech start", "speech: texto4");
        await tester.ForwardAsync("speech end", "thought start", "thought: texto5");
        await tester.ForwardAsync("thought: texto6");
        await tester.ForwardEndAsync("thought end", "person leave: clodoaldo");

        await tester.BackwardAsync("person enter: clodoaldo", "thought start", "thought: texto6");
        await tester.BackwardAsync("thought: texto5");
        await tester.BackwardAsync("thought end", "speech start", "speech: texto4");
        await tester.BackwardAsync("speech end", "thought start", "thought: texto3");
        await tester.BackwardAsync("thought end", "speech start", "speech: texto2");
        await tester.BackwardAsync("speech: texto1");
        await tester.BackwardEndAsync("speech end", "person leave: clodoaldo");
    }

    [Fact]
    public async Task SimpleTalk()
    {
        var xml =
@"<storyboard>
    <protagonist>jane</protagonist>
    <person>jane</person>
    <speech>texto1</speech>

    <person>clara</person>
    <speech>texto2</speech>
    <speech>texto3</speech>

    <person>jane</person>
    <thought>texto4</thought>
    <thought>texto5</thought>

    <person>clara</person>
    <thought>texto6</thought>
</storyboard>";

        var tester = await StoryboardEventTester.CreateAsync(xml);

        await tester.ForwardAsync("protagonist: jane", "person* enter: jane", "speech* start", "speech*: texto1");
        await tester.ForwardAsync("speech* end", "person* leave: jane", "person enter: clara", "speech start", "speech: texto2");
        await tester.ForwardAsync("speech: texto3");
        await tester.ForwardAsync("speech end", "person leave: clara", "person* enter: jane", "thought* start", "thought*: texto4");
        await tester.ForwardAsync("thought*: texto5");
        await tester.ForwardAsync("thought* end", "person* leave: jane", "person enter: clara", "thought start", "thought: texto6");
        await tester.ForwardEndAsync("thought end", "person leave: clara", "protagonist undefined");

        await tester.BackwardAsync("protagonist: jane", "person enter: clara", "thought start", "thought: texto6");
        await tester.BackwardAsync("thought end", "person leave: clara", "person* enter: jane", "thought* start", "thought*: texto5");
        await tester.BackwardAsync("thought*: texto4");
        await tester.BackwardAsync("thought* end", "person* leave: jane", "person enter: clara", "speech start", "speech: texto3");
        await tester.BackwardAsync("speech: texto2");
        await tester.BackwardAsync("speech end", "person leave: clara", "person* enter: jane", "speech* start", "speech*: texto1");
        await tester.BackwardEndAsync("speech* end", "person* leave: jane", "protagonist undefined");
    }
}
