namespace IS.Reading.StoryboardItems
{
    public class BackgroundItemTests
    {
        private readonly StoryboardBlock sut = new(null);
        private readonly StoryContext context = new();

        public BackgroundItemTests()
        {
            sut.ForwardQueue.Enqueue(new BackgroundItem("img1", null));
            sut.ForwardQueue.Enqueue(new BackgroundItem("img2", null));
            sut.ForwardQueue.Enqueue(new BackgroundItem("img3", null));
        }

        [Fact]
        public async Task BackAndForth1()
        {
            Check("", false);

            for (var n = 1; n <= 3; n++)
            {
                await NextAsync("img1");
                await NextAsync("img2");
                await NextAsync("img3");
                await NextAsync("img3", false);
                await PreviousAsync("img2");
                await PreviousAsync("img1");
                await PreviousAsync("");
                await PreviousAsync("", false);
            }
        }

        [Fact]
        public async Task BackAndForth2()
        {
            Check("", false);

            for (var n = 1; n <= 3; n++)
            {
                await NextAsync("img1");
                await PreviousAsync("");
                await PreviousAsync("", false);
                await NextAsync("img1");
                await NextAsync("img2");
                await PreviousAsync("img1");
                await PreviousAsync("");
                await PreviousAsync("", false);
                await NextAsync("img1");
                await NextAsync("img2");
                await NextAsync("img3");
                await PreviousAsync("img2");
                await PreviousAsync("img1");
                await PreviousAsync("");
                await PreviousAsync("", false);
                await NextAsync("img1");
                await NextAsync("img2");
                await NextAsync("img3");
                await NextAsync("img3", false);
                await PreviousAsync("img2");
                await PreviousAsync("img1");
                await PreviousAsync("");
                await PreviousAsync("", false);
            }
        }

        [Fact]
        public async Task BackAndForth3()
        {
            Check("", false);

            for (var n = 1; n <= 3; n++)
            {
                await NextAsync("img1");
                await PreviousAsync("");
                await NextAsync("img1");
                await NextAsync("img2");
                await PreviousAsync("img1");
                await NextAsync("img2");
                await NextAsync("img3");
                await PreviousAsync("img2");
                await NextAsync("img3");
                await NextAsync("img3", false);
                await PreviousAsync("img2");
                await NextAsync("img3");
                await PreviousAsync("img2");
                await PreviousAsync("img1");
                await NextAsync("img2");
                await PreviousAsync("img1");
                await PreviousAsync("");
                await NextAsync("img1");
                await PreviousAsync("");
                await PreviousAsync("", false);
            }
        }

        private async Task NextAsync(string imageName, bool mustHaveCurrent = true)
        {
            await sut.MoveNextAsync(context);
            Check(imageName, mustHaveCurrent);
        }

        private async Task PreviousAsync(string imageName, bool mustHaveCurrent = true)
        {
            await sut.MovePreviousAsync(context);
            Check(imageName, mustHaveCurrent);
        }

        private void Check(string imageName, bool mustHaveCurrent = true)
        {
            context.State[Keys.BackgroundImage].Should().Be(imageName);

            if (mustHaveCurrent)
                sut.Current.Should().BeOfType<BackgroundItem>().Which.ImageName.Should().Be(imageName);
            else
                sut.Current.Should().BeNull();
        }
    }
}
