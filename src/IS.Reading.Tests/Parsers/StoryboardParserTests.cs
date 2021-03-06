﻿using FluentAssertions;
using IS.Reading.StoryboardItems;
using System;
using System.Linq;
using Xunit;

namespace IS.Reading.Parsers
{
    public class StoryboardParserTests
    {
        private Storyboard Load(string resourceName)
            => StoryboardParser.Parse(this.GetResourceStream(resourceName));

        [Fact]
        public void SimpleElements()
        {
            var target = Load("SimpleElements.xml");

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
                item.Increment.ToString().Should().Be("var3+2");
            }
        }

        [Fact]
        public void SimpleNarration()
        {
            var target = Load("SimpleNarration.xml");

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
            var target = Load("SimpleTutorial.xml");

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
            var target = Load("SimpleProtagonist.xml");

            target.Get<BackgroundItem>().ImageName.Should().Be("imagem");

            var protagonist = target.Get<ProtagonistItem>();
            protagonist.Get<VarSetItem>().Name.Should().Be("var1");
            protagonist.Get<VarSetItem>().Name.Should().Be("var2");
            {
                var thought = protagonist.Get<ProtagonistThoughtItem>();
                thought.Get<ProtagonistBumpItem>().GetSingle<ProtagonistThoughtTextItem>().Text.Should().Be("Pensamento 1");
                thought.GetSingle<ProtagonistThoughtTextItem>().Text.Should().Be("Pensamento 2");
            }
            {
                var emotion = protagonist.GetSingle<ProtagonistMoodItem>();
                emotion.Get<VarSetItem>().Name.Should().Be("var3");
                emotion.Get<VarSetItem>().Name.Should().Be("var4");
                var speech = emotion.GetSingle<ProtagonistSpeechItem>();
                speech.Get<ProtagonistBumpItem>().GetSingle<ProtagonistSpeechTextItem>().Text.Should().Be("Fala 1");
                speech.GetSingle<ProtagonistSpeechTextItem>().Text.Should().Be("Fala 2");
            }

            target.GetSingle<MusicItem>().MusicName.Should().Be("musica");
        }

        [Fact]
        public void SimpleProtagonist2()
        {
            var target = Load("SimpleProtagonist2.xml");

            target.Get<MusicItem>().MusicName.Should().Be("musica");

            var protagonist = target.Get<ProtagonistItem>();
            protagonist.Get<VarSetItem>().Name.Should().Be("var1");
            protagonist.Get<VarSetItem>().Name.Should().Be("var2");
            {
                var block = protagonist.Get<ProtagonistSpeechItem>();
                block.Get<ProtagonistBumpItem>().GetSingle<ProtagonistSpeechTextItem>().Text.Should().Be("Fala 1");
                block.GetSingle<ProtagonistSpeechTextItem>().Text.Should().Be("Fala 2");
            }
            {
                var emotion = protagonist.GetSingle<ProtagonistMoodItem>();
                emotion.Get<VarSetItem>().Name.Should().Be("var3");
                emotion.Get<VarSetItem>().Name.Should().Be("var4");
                var block = emotion.GetSingle<ProtagonistThoughtItem>();
                block.Get<ProtagonistBumpItem>().GetSingle<ProtagonistThoughtTextItem>().Text.Should().Be("Pensamento 1");
                block.GetSingle<ProtagonistThoughtTextItem>().Text.Should().Be("Pensamento 2");
            }

            target.GetSingle<BackgroundItem>().ImageName.Should().Be("imagem");
        }

        [Fact]
        public void SimpleInterlocutor()
        {
            var target = Load("SimpleInterlocutor.xml");

            target.Get<BackgroundItem>().ImageName.Should().Be("imagem");

            var interlocutor = target.Get<InterlocutorItem>();
            interlocutor.Name.Should().Be("sulana");

            interlocutor.Get<VarSetItem>().Name.Should().Be("var1");
            interlocutor.Get<VarSetItem>().Name.Should().Be("var2");
            {
                var thought = interlocutor.Get<InterlocutorThoughtItem>();
                thought.Get<InterlocutorBumpItem>().GetSingle<InterlocutorThoughtTextItem>().Text.Should().Be("Pensamento 1");
                thought.Get<InterlocutorThoughtTextItem>().Text.Should().Be("Pensamento 2");
            }
            {
                var emotion = interlocutor.GetSingle<InterlocutorMoodItem>();
                emotion.Get<VarSetItem>().Name.Should().Be("var3");
                emotion.Get<VarSetItem>().Name.Should().Be("var4");
                var speech = emotion.GetSingle<InterlocutorSpeechItem>();
                speech.Get<InterlocutorBumpItem>().GetSingle<InterlocutorSpeechTextItem>().Text.Should().Be("Fala 1");
                speech.GetSingle<InterlocutorSpeechTextItem>().Text.Should().Be("Fala 2");
            }

            target.GetSingle<MusicItem>().MusicName.Should().Be("musica");
        }

        [Fact]
        public void SimpleInterlocutor2()
        {
            var target = Load("SimpleInterlocutor2.xml");

            target.Get<MusicItem>().MusicName.Should().Be("musica");

            var interlocutor = target.Get<InterlocutorItem>();
            interlocutor.Name.Should().Be("belisar");

            interlocutor.Get<VarSetItem>().Name.Should().Be("var1");
            interlocutor.Get<VarSetItem>().Name.Should().Be("var2");
            {
                var block = interlocutor.Get<InterlocutorSpeechItem>();
                block.Get<InterlocutorSpeechTextItem>().Text.Should().Be("Fala 1");
                block.GetSingle<InterlocutorBumpItem>().GetSingle<InterlocutorSpeechTextItem>().Text.Should().Be("Fala 2");
            }
            {
                var emotion = interlocutor.GetSingle<InterlocutorMoodItem>();
                emotion.Get<VarSetItem>().Name.Should().Be("var3");
                emotion.Get<VarSetItem>().Name.Should().Be("var4");
                var block = emotion.GetSingle<InterlocutorThoughtItem>();
                block.Get<InterlocutorThoughtTextItem>().Text.Should().Be("Pensamento 1");
                block.GetSingle<InterlocutorBumpItem>().GetSingle<InterlocutorThoughtTextItem>().Text.Should().Be("Pensamento 2");
            }

            target.GetSingle<BackgroundItem>().ImageName.Should().Be("imagem");
        }

        [Fact]
        public void SimplePrompt()
        {
            var target = Load("SimplePrompt.xml");

            target.Get<BackgroundItem>().ImageName.Should().Be("imagem");

            var protagonist = target.Get<ProtagonistItem>();
            protagonist.Get<ProtagonistSpeechItem>().GetSingle<ProtagonistSpeechTextItem>().Text.Should().Be("Fala 1");

            var prompt = protagonist.Get<PromptItem>();
            prompt.Prompt.Choices.Select(i => i.Value).Should().BeEquivalentTo("a", "b", "c");
            prompt.Prompt.Choices.Select(i => i.Text).Should().BeEquivalentTo("Opção 1", "Opção 2", "Opção 3");
            prompt.GetSingle<ProtagonistSpeechItem>().GetSingle<ProtagonistSpeechTextItem>().Text.Should().Be("Fala 2");

            protagonist.GetSingle<ProtagonistSpeechItem>().GetSingle<ProtagonistSpeechTextItem>().Text.Should().Be("Fala 3");

            target.GetSingle<MusicItem>().MusicName.Should().Be("musica");
        }

        [Fact]
        public void SimplePrompt2()
        {
            var target = Load("SimplePrompt2.xml");
            
            target.Get<BackgroundItem>().ImageName.Should().Be("imagem");

            var prompt = target.Get<PromptItem>();
            prompt.Prompt.Choices.Select(i => i.Value).Should().BeEquivalentTo("a", "b");
            prompt.Prompt.Choices.Select(i => i.Text).Should().BeEquivalentTo("Opção A", "Opção B");
            prompt.GetSingle<NarrationItem>().GetSingle<NarrationTextItem>().Text.Should().Be("Narração");

            target.GetSingle<MusicItem>().MusicName.Should().Be("musica");
        }

        [Fact]
        public void SimplePrompt3()
        {
            var target = Load("SimplePrompt3.xml");

            target.Get<TutorialItem>().GetSingle<TutorialTextItem>().Text.Should().Be("Tutorial 1");

            var prompt = target.Get<PromptItem>();
            prompt.Condition.ToString().Should().Be("condicao1[1:]");
            prompt.Prompt.Choices.Select(i => i.Value).Should().BeEquivalentTo("a", "b");
            prompt.Prompt.Choices.Select(i => i.Text).Should().BeEquivalentTo("Opção A", "Opção B");
            prompt.Prompt.Choices.Select(i => i.Condition.ToString()).Should().BeEquivalentTo("condicao2[1:]", "condicao3[1:]");
            prompt.GetSingle<TutorialItem>().GetSingle<TutorialTextItem>().Text.Should().Be("Tutorial 2");

            target.GetSingle<TutorialItem>().GetSingle<TutorialTextItem>().Text.Should().Be("Tutorial 3");
        }

        [Fact]
        public void SimplePrompt4()
        {
            var target = Load("SimplePrompt4.xml");

            target.Get<NarrationItem>().GetSingle<NarrationTextItem>().Text.Should().Be("Narração 1");

            var prompt = target.Get<PromptItem>();
            prompt.Prompt.TimeLimit.Should().Be(TimeSpan.FromSeconds(5));
            prompt.Prompt.RandomOrder.Should().BeTrue();
            prompt.Prompt.DefaultChoice.Should().Be("b");
            prompt.Prompt.Choices.Select(i => i.Value).Should().BeEquivalentTo("a", "b");
            prompt.Prompt.Choices.Select(i => i.Text).Should().BeEquivalentTo("Opção A", "Opção B");
            prompt.Prompt.Choices.Select(i => i.Tip.ToString()).Should().BeEquivalentTo("Coragem 1", "Força 2");
            prompt.GetSingle<NarrationItem>().GetSingle<NarrationTextItem>().Text.Should().Be("Narração 2");

            target.GetSingle<NarrationItem>().GetSingle<NarrationTextItem>().Text.Should().Be("Narração 3");
        }

        [Fact]
        public void ProtagonistReward()
        {
            var target = Load("ProtagonistReward.xml");

            var protagonist = target.GetSingle<ProtagonistItem>();
            protagonist.Get<ProtagonistRewardItem>().Increment.ToString().Should().Be("v1+1");
            
            var speech = protagonist.Get<ProtagonistSpeechItem>();
            speech.Get<ProtagonistSpeechTextItem>().Text.Should().Be("Fala 1");
            speech.Get<ProtagonistRewardItem>().Increment.ToString().Should().Be("v2-1");
            speech.GetSingle<ProtagonistSpeechTextItem>().Text.Should().Be("Fala 2");

            var mood = protagonist.GetSingle<ProtagonistMoodItem>();
            mood.Get<ProtagonistRewardItem>().Increment.ToString().Should().Be("v3+2");

            var thought = mood.GetSingle<ProtagonistThoughtItem>();
            thought.Get<ProtagonistThoughtTextItem>().Text.Should().Be("Pensamento 1");
            thought.Get<ProtagonistRewardItem>().Increment.ToString().Should().Be("v4-2");
            thought.GetSingle<ProtagonistThoughtTextItem>().Text.Should().Be("Pensamento 2");
        }

        [Fact]
        public void InterlocutorReward()
        {
            var target = Load("InterlocutorReward.xml");

            var interlocutor = target.GetSingle<InterlocutorItem>();
            interlocutor.Name.Should().Be("sulana");
            interlocutor.Get<InterlocutorRewardItem>().Increment.ToString().Should().Be("v1+1");

            var speech = interlocutor.Get<InterlocutorSpeechItem>();
            speech.Get<InterlocutorSpeechTextItem>().Text.Should().Be("Fala 1");
            speech.Get<InterlocutorRewardItem>().Increment.ToString().Should().Be("v2-1");
            speech.GetSingle<InterlocutorSpeechTextItem>().Text.Should().Be("Fala 2");

            var mood = interlocutor.GetSingle<InterlocutorMoodItem>();
            mood.Get<InterlocutorRewardItem>().Increment.ToString().Should().Be("v3+2");

            var thought = mood.GetSingle<InterlocutorThoughtItem>();
            thought.Get<InterlocutorThoughtTextItem>().Text.Should().Be("Pensamento 1");
            thought.Get<InterlocutorRewardItem>().Increment.ToString().Should().Be("v4-2");
            thought.GetSingle<InterlocutorThoughtTextItem>().Text.Should().Be("Pensamento 2");
        }
    }
}
