using IS.Reading.State;

namespace IS.Reading.Events;

public class BackgroundChangeEvent(
    IBackgroundState state,
    BackgroundAnimation animation = BackgroundAnimation.None,
    string? flashColor = null
) : IBackgroundChangeEvent
{
    public IBackgroundState State { get; } = state;
    public BackgroundAnimation Animation { get; } = animation;
    public string? FlashColor { get; } = flashColor;

    public override string? ToString()
        => State.ToString() + AnimationSuffix();

    private string AnimationSuffix()
        => Animation switch
        {
            BackgroundAnimation.FadeIn   => " [fadein]",
            BackgroundAnimation.Zoom     => " [zoom]",
            BackgroundAnimation.Dissolve => " [dissolve]",
            BackgroundAnimation.Flash when FlashColor is not null => $" [flash:{FlashColor}]",
            BackgroundAnimation.Flash    => " [flash]",
            _                            => string.Empty
        };
}
