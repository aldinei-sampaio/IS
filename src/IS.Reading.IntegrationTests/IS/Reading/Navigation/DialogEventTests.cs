﻿namespace IS.Reading.Navigation;

public class DialogEventTests
{
    [Fact]
    public async Task TutorialAndNarration()
    {
        var stb =
@"' Storybasic 1.0
! texto1
! texto2
> texto3
> texto4
! texto5
> texto6";

        var tester = await StoryboardEventTester.CreateAsync(stb);

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
    public async Task MainCharacterSpeechAndThought()
    {
        var stb =
@"' Storybasic 1.0
mc joara
@ joara
- texto1
- texto2
* texto3
- texto4
* texto5
* texto6";

        var tester = await StoryboardEventTester.CreateAsync(stb);

        await tester.ForwardAsync("mc: joara", "person* enter: joara", "speech* start", "speech*: texto1");
        await tester.ForwardAsync("speech*: texto2");
        await tester.ForwardAsync("speech* end", "thought* start", "thought*: texto3");
        await tester.ForwardAsync("thought* end", "speech* start", "speech*: texto4");
        await tester.ForwardAsync("speech* end", "thought* start", "thought*: texto5");
        await tester.ForwardAsync("thought*: texto6");
        await tester.ForwardEndAsync("thought* end", "person* leave: joara", "mc unset");

        await tester.BackwardAsync("mc: joara", "person* enter: joara", "thought* start", "thought*: texto6");
        await tester.BackwardAsync("thought*: texto5");
        await tester.BackwardAsync("thought* end", "speech* start", "speech*: texto4");
        await tester.BackwardAsync("speech* end", "thought* start", "thought*: texto3");
        await tester.BackwardAsync("thought* end", "speech* start", "speech*: texto2");
        await tester.BackwardAsync("speech*: texto1");
        await tester.BackwardEndAsync("speech* end", "person* leave: joara", "mc unset");
    }

    [Fact]
    public async Task InterlocutorSpeechAndThought()
    {
        var stb =
@"' Storybasic 1.0
@ clodoaldo
- texto1
- texto2
* texto3
- texto4
* texto5
* texto6";
        
        var tester = await StoryboardEventTester.CreateAsync(stb);

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
        var stb =
@"' Storybasic 1.0
mc jane

@ jane
- texto1

@ clara
- texto2
- texto3

@ jane
* texto4
* texto5

@ clara
* texto6";

        var tester = await StoryboardEventTester.CreateAsync(stb);

        await tester.ForwardAsync("mc: jane", "person* enter: jane", "speech* start", "speech*: texto1");
        await tester.ForwardAsync("speech* end", "person* leave: jane", "person enter: clara", "speech start", "speech: texto2");
        await tester.ForwardAsync("speech: texto3");
        await tester.ForwardAsync("speech end", "person leave: clara", "person* enter: jane", "thought* start", "thought*: texto4");
        await tester.ForwardAsync("thought*: texto5");
        await tester.ForwardAsync("thought* end", "person* leave: jane", "person enter: clara", "thought start", "thought: texto6");
        await tester.ForwardEndAsync("thought end", "person leave: clara", "mc unset");

        await tester.BackwardAsync("mc: jane", "person enter: clara", "thought start", "thought: texto6");
        await tester.BackwardAsync("thought end", "person leave: clara", "person* enter: jane", "thought* start", "thought*: texto5");
        await tester.BackwardAsync("thought*: texto4");
        await tester.BackwardAsync("thought* end", "person* leave: jane", "person enter: clara", "speech start", "speech: texto3");
        await tester.BackwardAsync("speech: texto2");
        await tester.BackwardAsync("speech end", "person leave: clara", "person* enter: jane", "speech* start", "speech*: texto1");
        await tester.BackwardEndAsync("speech* end", "person* leave: jane", "mc unset");
    }

    [Fact]
    public async Task MainCharacterMood()
    {
        var stb =
@"' Storybasic 1.0
mc jane

@ jane
- texto1

# surprised
- texto2
- texto3

# angry
* texto5";

        var tester = await StoryboardEventTester.CreateAsync(stb);

        await TestMood(tester);
        await TestMood(tester);
        await TestMood(tester);

        static async Task TestMood(StoryboardEventTester tester)
        {
            await tester.ForwardAsync("mc: jane", "person* enter: jane", "speech* start", "speech*: texto1");
            await tester.ForwardAsync("mood* surprised: jane", "speech*: texto2");
            await tester.ForwardAsync("speech*: texto3");
            await tester.ForwardAsync("speech* end", "mood* angry: jane", "thought* start", "thought*: texto5");
            await tester.ForwardEndAsync("thought* end", "person* leave: jane", "mc unset");

            await tester.BackwardAsync("mc: jane", "person* enter: jane", "mood* angry: jane", "thought* start", "thought*: texto5");
            await tester.BackwardAsync("thought* end", "mood* surprised: jane", "speech* start", "speech*: texto3");
            await tester.BackwardAsync("speech*: texto2");
            await tester.BackwardAsync("mood* normal: jane", "speech*: texto1");
            await tester.BackwardEndAsync("speech* end", "person* leave: jane", "mc unset");
        }
    }

    [Fact]
    public async Task InterlocutorMood()
    {
        var stb =
@"' Storybasic 1.0
@ lana
# normal
- texto1

# surprised
- texto2
- texto3

# angry
* texto5";

        var tester = await StoryboardEventTester.CreateAsync(stb);

        await TestMood(tester);
        await TestMood(tester);
        await TestMood(tester);

        static async Task TestMood(StoryboardEventTester tester)
        {
            await tester.ForwardAsync("person enter: lana", "speech start", "speech: texto1");
            await tester.ForwardAsync("mood surprised: lana", "speech: texto2");
            await tester.ForwardAsync("speech: texto3");
            await tester.ForwardAsync("speech end", "mood angry: lana", "thought start", "thought: texto5");
            await tester.ForwardEndAsync("thought end", "person leave: lana");

            await tester.BackwardAsync("person enter: lana", "mood angry: lana", "thought start", "thought: texto5");
            await tester.BackwardAsync("thought end", "mood surprised: lana", "speech start", "speech: texto3");
            await tester.BackwardAsync("speech: texto2");
            await tester.BackwardAsync("mood normal: lana", "speech: texto1");
            await tester.BackwardEndAsync("speech end", "person leave: lana");
        }
    }

    [Fact]
    public async Task Title()
    {
        var stb =
@"' Storybasic 1.0
@ lana
title ???
- texto1

# surprised
title Lana
- texto2
- texto3

# angry
* texto5";

        var tester = await StoryboardEventTester.CreateAsync(stb);

        await TestMood(tester);
        await TestMood(tester);
        await TestMood(tester);

        static async Task TestMood(StoryboardEventTester tester)
        {
            await tester.ForwardAsync("person enter: lana", "title: ???", "speech start", "speech: texto1");
            await tester.ForwardAsync("mood surprised: lana", "title: Lana", "speech: texto2");
            await tester.ForwardAsync("speech: texto3");
            await tester.ForwardAsync("speech end", "mood angry: lana", "thought start", "thought: texto5");
            await tester.ForwardEndAsync("thought end", "person leave: lana");

            await tester.BackwardAsync("person enter: lana", "mood angry: lana", "title: Lana", "thought start", "thought: texto5");
            await tester.BackwardAsync("thought end", "mood surprised: lana", "speech start", "speech: texto3");
            await tester.BackwardAsync("speech: texto2");
            await tester.BackwardAsync("mood normal: lana", "title: ???", "speech: texto1");
            await tester.BackwardEndAsync("speech end", "person leave: lana");
        }
    }
}
