using IS.Reading.State;

namespace IS.Reading.Events;

public class BackgroundChangeEvent(IBackgroundState state) : IBackgroundChangeEvent
{
    public IBackgroundState State { get; } = state;

    public override string? ToString()
        => State.ToString();
}
