using FluentAssertions;
using IS.Reading.StoryboardItems;
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

            Get<ProtagonistChangeItem>(target).Name.Should().Be("sulana");
            Get<MusicItem>(target).MusicName.Should().Be("never_look_back");
            Get<BackgroundItem>(target).ImageName.Should().Be("carmim");
            {
                var item = Get<VarSetItem>(target);
                item.Name.Should().Be("var1");
                item.Value.Should().Be(0);
            }
            {
                var item = Get<VarSetItem>(target);
                item.Name.Should().Be("var2");
                item.Value.Should().Be(1);
            }
            Get<PauseItem>(target);
            {
                var item = Get<VarIncrementItem>(target);
                item.Name.Should().Be("var3");
                item.Increment.Should().Be(2);
            }
        }

        [Fact]
        public void SimpleNarration()
        {
            var target = StoryboardParser.Load(Resources.SimpleNarration);

            target.Root.ForwardQueue.Count.Should().Be(3);

            Get<BackgroundItem>(target).ImageName.Should().Be("imagem");

            var narration = Get<NarrationItem>(target);
            narration.Block.ForwardQueue.Count.Should().Be(3);
            Get<NarrationTextItem>(narration).Text.Should().Be("Primeira fala");
            Get<NarrationTextItem>(narration).Text.Should().Be("Segunda fala");
            Get<NarrationTextItem>(narration).Text.Should().Be("Terceira fala");

            Get<MusicItem>(target).MusicName.Should().Be("musica");
        }

        private T Get<T>(Storyboard storyboard) where T : IStoryboardItem
            => Get<T>(storyboard.Root);

        private T Get<T>(IStoryboardItem item) where T : IStoryboardItem
            => Get<T>(item.Block);

        private T Get<T>(StoryboardBlock block) where T : IStoryboardItem
        {
            var item = block.ForwardQueue.Dequeue();
            item.Should().BeOfType<T>();
            return (T)item;
        }
    }
}
