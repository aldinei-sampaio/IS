using FluentAssertions;
using IS.Reading.StoryboardItems;
using System;
using System.Linq;
using Xunit;

namespace IS.Reading.Parsers
{
    public class StoryboardParserTests
    {
        [Fact]
        public void SimpleElements()
        {
            var target = StoryboardParser.Load(Resources.SimpleElements);

            target.Root.ForwardQueue.Count.Should().Be(7);

            target.Get<ProtagonistChangeItem>().Name.Should().Be("sulana");
            target.Get<MusicItem>().MusicName.Should().Be("never_look_back");
            target.Get<BackgroundItem>().ImageName.Should().Be("carmim");
            {
                var item = target.Get<VarSetItem>();
                item.Name.Should().Be("var1");
                item.Value.Should().Be(0);
            }
            {
                var item = target.Get<VarSetItem>();
                item.Name.Should().Be("var2");
                item.Value.Should().Be(1);
            }
            target.Get<PauseItem>();
            {
                var item = target.Get<VarIncrementItem>();
                item.Name.Should().Be("var3");
                item.Increment.Should().Be(2);
            }
        }

        [Fact]
        public void SimpleNarration()
        {
            var target = StoryboardParser.Load(Resources.SimpleNarration);

            target.Root.ForwardQueue.Count.Should().Be(3);

            target.Get<BackgroundItem>().ImageName.Should().Be("imagem");

            var narration = target.Get<NarrationItem>();
            narration.Block.ForwardQueue.Count.Should().Be(3);
            narration.Get<NarrationTextItem>().Text.Should().Be("Primeira fala");
            narration.Get<NarrationTextItem>().Text.Should().Be("Segunda fala");
            narration.Get<NarrationTextItem>().Text.Should().Be("Terceira fala");

            target.Get<MusicItem>().MusicName.Should().Be("musica");
        }

        [Fact]
        public void SimpleTutorial()
        {
            var target = StoryboardParser.Load(Resources.SimpleTutorial);

            target.Root.ForwardQueue.Count.Should().Be(3);

            target.Get<BackgroundItem>().ImageName.Should().Be("imagem");

            var narration = target.Get<TutorialItem>();
            narration.Block.ForwardQueue.Count.Should().Be(3);
            narration.Get<TutorialTextItem>().Text.Should().Be("Primeira fala");
            narration.Get<TutorialTextItem>().Text.Should().Be("Segunda fala");
            narration.Get<TutorialTextItem>().Text.Should().Be("Terceira fala");

            target.Get<MusicItem>().MusicName.Should().Be("musica");
        }

        [Fact]
        public void SimpleProtagonist()
        {
            var target = StoryboardParser.Load(Resources.SimpleProtagonist);

            target.Root.ForwardQueue.Count.Should().Be(3);

            target.Get<BackgroundItem>().ImageName.Should().Be("imagem");

            var protagonist = target.Get<ProtagonistItem>();
            protagonist.Block.ForwardQueue.Count.Should().Be(5);
            protagonist.Get<ProtagonistBumpItem>();
            protagonist.Get<VarSetItem>().Name.Should().Be("var1");
            protagonist.Get<VarSetItem>().Name.Should().Be("var2");
            {
                var thought = protagonist.Get<ProtagonistThoughtItem>();
                thought.Block.ForwardQueue.Count.Should().Be(2);
                thought.Get<ProtagonistThoughtTextItem>().Text.Should().Be("Pensamento 1");
                thought.Get<ProtagonistThoughtTextItem>().Text.Should().Be("Pensamento 2");
            }
            {
                var emotion = protagonist.Get<ProtagonistMoodItem>();
                emotion.Block.ForwardQueue.Count.Should().Be(4);
                emotion.Get<ProtagonistBumpItem>();
                emotion.Get<VarSetItem>().Name.Should().Be("var3");
                emotion.Get<VarSetItem>().Name.Should().Be("var4");
                var speech = emotion.Get<ProtagonistSpeechItem>();
                speech.Block.ForwardQueue.Count.Should().Be(2);
                speech.Get<ProtagonistSpeechTextItem>().Text.Should().Be("Fala 1");
                speech.Get<ProtagonistSpeechTextItem>().Text.Should().Be("Fala 2");
            }

            target.Get<MusicItem>().MusicName.Should().Be("musica");
        }

        [Fact]
        public void SimpleProtagonist2()
        {
            var target = StoryboardParser.Load(Resources.SimpleProtagonist2);

            target.Root.ForwardQueue.Count.Should().Be(3);

            target.Get<MusicItem>().MusicName.Should().Be("musica");

            var protagonist = target.Get<ProtagonistItem>();
            protagonist.Block.ForwardQueue.Count.Should().Be(5);
            protagonist.Get<ProtagonistBumpItem>();
            protagonist.Get<VarSetItem>().Name.Should().Be("var1");
            protagonist.Get<VarSetItem>().Name.Should().Be("var2");
            {
                var block = protagonist.Get<ProtagonistSpeechItem>();
                block.Block.ForwardQueue.Count.Should().Be(2);
                block.Get<ProtagonistSpeechTextItem>().Text.Should().Be("Fala 1");
                block.Get<ProtagonistSpeechTextItem>().Text.Should().Be("Fala 2");
            }
            {
                var emotion = protagonist.Get<ProtagonistMoodItem>();
                emotion.Block.ForwardQueue.Count.Should().Be(4);
                emotion.Get<ProtagonistBumpItem>();
                emotion.Get<VarSetItem>().Name.Should().Be("var3");
                emotion.Get<VarSetItem>().Name.Should().Be("var4");
                var block = emotion.Get<ProtagonistThoughtItem>();
                block.Block.ForwardQueue.Count.Should().Be(2);
                block.Get<ProtagonistThoughtTextItem>().Text.Should().Be("Pensamento 1");
                block.Get<ProtagonistThoughtTextItem>().Text.Should().Be("Pensamento 2");
            }

            target.Get<BackgroundItem>().ImageName.Should().Be("imagem");
        }

        [Fact]
        public void SimplePrompt()
        {
            var target = StoryboardParser.Load(Resources.SimplePrompt);

            target.Root.ForwardQueue.Count.Should().Be(3);

            target.Get<BackgroundItem>().ImageName.Should().Be("imagem");

            var protagonist = target.Get<ProtagonistItem>();
            protagonist.Block.ForwardQueue.Count.Should().Be(3);
            protagonist.Get<ProtagonistSpeechItem>().GetSingle<ProtagonistSpeechTextItem>().Text.Should().Be("Fala 1");

            var prompt = protagonist.Get<PromptItem>();
            prompt.Prompt.Choices.Select(i => i.Value).Should().BeEquivalentTo("a", "b", "c");
            prompt.Prompt.Choices.Select(i => i.Text).Should().BeEquivalentTo("Opção 1", "Opção 2", "Opção 3");
            prompt.GetSingle<ProtagonistSpeechItem>().GetSingle<ProtagonistSpeechTextItem>().Text.Should().Be("Fala 2");

            protagonist.Get<ProtagonistSpeechItem>().GetSingle<ProtagonistSpeechTextItem>().Text.Should().Be("Fala 3");

            target.Get<MusicItem>().MusicName.Should().Be("musica");
        }

        [Fact]
        public void SimplePrompt2()
        {
            var target = StoryboardParser.Load(Resources.SimplePrompt2);

            target.Root.ForwardQueue.Count.Should().Be(3);

            target.Get<BackgroundItem>().ImageName.Should().Be("imagem");

            var prompt = target.Get<PromptItem>();
            prompt.Prompt.Choices.Select(i => i.Value).Should().BeEquivalentTo("a", "b");
            prompt.Prompt.Choices.Select(i => i.Text).Should().BeEquivalentTo("Opção A", "Opção B");
            prompt.GetSingle<NarrationItem>().GetSingle<NarrationTextItem>().Text.Should().Be("Narração");

            target.Get<MusicItem>().MusicName.Should().Be("musica");
        }

        [Fact]
        public void SimplePrompt3()
        {
            var target = StoryboardParser.Load(Resources.SimplePrompt3);

            target.Root.ForwardQueue.Count.Should().Be(3);

            target.Get<TutorialItem>().GetSingle<TutorialTextItem>().Text.Should().Be("Tutorial 1");

            var prompt = target.Get<PromptItem>();
            prompt.Condition.ToString().Should().Be("condicao1[1:]");
            prompt.Prompt.Choices.Select(i => i.Value).Should().BeEquivalentTo("a", "b");
            prompt.Prompt.Choices.Select(i => i.Text).Should().BeEquivalentTo("Opção A", "Opção B");
            prompt.Prompt.Choices.Select(i => i.Condition.ToString()).Should().BeEquivalentTo("condicao2[1:]", "condicao3[1:]");
            prompt.GetSingle<TutorialItem>().GetSingle<TutorialTextItem>().Text.Should().Be("Tutorial 2");

            target.Get<TutorialItem>().GetSingle<TutorialTextItem>().Text.Should().Be("Tutorial 3");
        }

        [Fact]
        public void SimplePrompt4()
        {
            var target = StoryboardParser.Load(Resources.SimplePrompt4);

            target.Root.ForwardQueue.Count.Should().Be(3);

            target.Get<NarrationItem>().GetSingle<NarrationTextItem>().Text.Should().Be("Narração 1");

            var prompt = target.Get<PromptItem>();
            prompt.Prompt.TimeLimit.Should().Be(TimeSpan.FromSeconds(5));
            prompt.Prompt.RandomOrder.Should().BeTrue();
            prompt.Prompt.DefaultChoice.Should().Be("b");
            prompt.Prompt.Choices.Select(i => i.Value).Should().BeEquivalentTo("a", "b");
            prompt.Prompt.Choices.Select(i => i.Text).Should().BeEquivalentTo("Opção A", "Opção B");
            prompt.Prompt.Choices.Select(i => i.Tip.ToString()).Should().BeEquivalentTo("Coragem 1", "Força 2");
            prompt.GetSingle<NarrationItem>().GetSingle<NarrationTextItem>().Text.Should().Be("Narração 2");

            target.Get<NarrationItem>().GetSingle<NarrationTextItem>().Text.Should().Be("Narração 3");
        }

    }
}
