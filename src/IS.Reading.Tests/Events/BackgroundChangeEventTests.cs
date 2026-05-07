using IS.Reading.State;

namespace IS.Reading.Events;

public class BackgroundChangeEventTests
{
    [Fact]
    public void InitializationWithNoAnimation()
    {
        var state = A.Dummy<IBackgroundState>();
        A.CallTo(() => state.ToString()).Returns("bg left: img");

        var sut = new BackgroundChangeEvent(state);
        sut.State.Should().BeSameAs(state);
        sut.Animation.Should().Be(BackgroundAnimation.None);
        sut.FlashColor.Should().BeNull();
        sut.ToString().Should().Be("bg left: img");
    }

    [Theory]
    [InlineData(BackgroundAnimation.FadeIn,   null,    "bg left: img [fadein]")]
    [InlineData(BackgroundAnimation.Zoom,     null,    "bg left: img [zoom]")]
    [InlineData(BackgroundAnimation.Dissolve, null,    "bg left: img [dissolve]")]
    [InlineData(BackgroundAnimation.Flash,    null,    "bg left: img [flash]")]
    [InlineData(BackgroundAnimation.Flash,    "white", "bg left: img [flash:white]")]
    [InlineData(BackgroundAnimation.Flash,    "red",   "bg left: img [flash:red]")]
    public void ToStringWithAnimation(BackgroundAnimation animation, string? flashColor, string expected)
    {
        var state = A.Dummy<IBackgroundState>();
        A.CallTo(() => state.ToString()).Returns("bg left: img");

        var sut = new BackgroundChangeEvent(state, animation, flashColor);
        sut.Animation.Should().Be(animation);
        sut.FlashColor.Should().Be(flashColor);
        sut.ToString().Should().Be(expected);
    }
}
