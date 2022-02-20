using IS.Reading.Events;

namespace IS.Reading.Navigation;

public interface IStoryboard : IDisposable
{
    IEventSubscriber Events { get; }
    bool IsWaitingForChoice { get; }

    Task<bool> MoveAsync(bool forward);
    void SetChoice(string value);
    // TODO: Task LoadStateAsync(Stream stream);
    // TODO: Task SaveStateAsync(Stream stream);
}
