using IS.Reading.State;

namespace IS.Reading.Events;

public class BackgroundChangeEvent : IBackgroundChangeEvent
{
    public BackgroundChangeEvent(IBackgroundState state)
        => State = state;

    public IBackgroundState State { get; }
}
