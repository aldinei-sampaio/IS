using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS.Reading.StoryboardItems
{
    public class ConditionItemTests
    {
        private Storyboard Case1()
        {
            var sut = new Storyboard();

            var c1 = new ConditionItem(null);
            c1.Block.ForwardQueue.Enqueue(new BackgroundItem("img1", null));
            c1.Block.ForwardQueue.Enqueue(new PauseItem(null));
            sut.Root.ForwardQueue.Enqueue(c1);

            var c2 = new ConditionItem(null);
            c2.Block.ForwardQueue.Enqueue(new BackgroundItem("img2", null));
            c2.Block.ForwardQueue.Enqueue(new PauseItem(null));
            var c2a = new ConditionItem(null);
            c2a.Block.ForwardQueue.Enqueue(new BackgroundItem("img3", null));
            c2.Block.ForwardQueue.Enqueue(c2a);
            sut.Root.ForwardQueue.Enqueue(c2);
            sut.Root.ForwardQueue.Enqueue(new PauseItem(null));

            var c3 = new ConditionItem(null);
            var c3a = new ConditionItem(null);
            c3a.Block.ForwardQueue.Enqueue(new BackgroundItem("img4", null));
            c3.Block.ForwardQueue.Enqueue(c3a);
            c3.Block.ForwardQueue.Enqueue(new PauseItem(null));
            var c3b = new ConditionItem(null);
            c3b.Block.ForwardQueue.Enqueue(new BackgroundItem("img5", null));
            c3b.Block.ForwardQueue.Enqueue(new PauseItem(null));
            c3b.Block.ForwardQueue.Enqueue(new BackgroundItem("img6", null));
            c3.Block.ForwardQueue.Enqueue(c3b);
            var c3c = new ConditionItem(null);
            c3c.Block.ForwardQueue.Enqueue(new PauseItem(null));
            c3c.Block.ForwardQueue.Enqueue(new BackgroundItem("img7", null));
            c3.Block.ForwardQueue.Enqueue(c3c);
            c3.Block.ForwardQueue.Enqueue(new PauseItem(null));

            sut.Root.ForwardQueue.Enqueue(c3);

            return sut;
        }

        [Fact]
        public async Task BackAndForth1()
        {
            var sut = Case1();
            Check(sut, "");
            for (var n = 1; n <= 3; n++)
            {
                await AdvanceAsync(sut, "img1", "img2", "img3", "img4", "img5", "img6", "img7", "");
                await GoBackAsync(sut, "img7", "img6", "img5", "img4", "img3", "img2", "img1", "");
            }
        }

        [Fact]
        public async Task BackAndForth2()
        {
            var sut = Case1();

            Check(sut, "");

            await AdvanceAsync(sut, "img1");
            await GoBackAsync(sut, "");

            await AdvanceAsync(sut, "img1", "img2");
            await GoBackAsync(sut, "img1", "");

            await AdvanceAsync(sut, "img1", "img2", "img3");
            await GoBackAsync(sut, "img2", "img1", "");

            await AdvanceAsync(sut, "img1", "img2", "img3", "img4");
            await GoBackAsync(sut, "img3", "img2", "img1", "");

            await AdvanceAsync(sut, "img1", "img2", "img3", "img4", "img5");
            await GoBackAsync(sut, "img4", "img3", "img2", "img1", "");

            await AdvanceAsync(sut, "img1", "img2", "img3", "img4", "img5", "img6");
            await GoBackAsync(sut, "img5", "img4", "img3", "img2", "img1", "");

            await AdvanceAsync(sut, "img1", "img2", "img3", "img4", "img5", "img6", "img7");
            await GoBackAsync(sut, "img6", "img5", "img4", "img3", "img2", "img1", "");

            await AdvanceAsync(sut, "img1", "img2", "img3", "img4", "img5", "img6", "img7", "");

            await GoBackAsync(sut, "img7");
            await AdvanceAsync(sut, "");

            await GoBackAsync(sut, "img7", "img6");
            await AdvanceAsync(sut, "img6", "");

            await GoBackAsync(sut, "img7", "img6", "img5");
            await AdvanceAsync(sut, "img5", "img6", "");

            await GoBackAsync(sut, "img7", "img6", "img5", "img4");
            await AdvanceAsync(sut, "img4", "img5", "img6", "");

            await GoBackAsync(sut, "img7", "img6", "img5", "img4", "img3");
            await AdvanceAsync(sut, "img3", "img4", "img5", "img6", "");

            await GoBackAsync(sut, "img7", "img6", "img5", "img4", "img3", "img2");
            await AdvanceAsync(sut, "img2", "img3", "img4", "img5", "img6", "");

            await GoBackAsync(sut, "img7", "img6", "img5", "img4", "img3", "img2", "img1");
            await AdvanceAsync(sut, "img1", "img2", "img3", "img4", "img5", "img6", "");
        }

        private static async Task AdvanceAsync(Storyboard sut, params string[] names)
        {
            foreach (var name in names)
                await NextAsync(sut, name);
        }

        private static async Task GoBackAsync(Storyboard sut, params string[] names)
        {
            foreach (var name in names)
                await PreviousAsync(sut, name);
        }

        private static async Task NextAsync(Storyboard sut, string imageName)
        {
            (await sut.MoveNextAsync()).Should().Be(imageName != "");
            Check(sut, imageName);
        }

        private static async Task PreviousAsync(Storyboard sut, string imageName)
        {
            (await sut.MovePreviousAsync()).Should().Be(imageName != "");
            Check(sut, imageName);
        }

        private static void Check(Storyboard sut, string imageName)
        {
            sut.Context.State[Keys.BackgroundImage].Should().Be(imageName);
        }

    }
}
