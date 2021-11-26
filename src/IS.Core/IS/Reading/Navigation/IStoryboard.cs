using IS.Reading.Events;

namespace IS.Reading.Navigation;

public interface IStoryboard : IDisposable
{
    IEventSubscriber Events { get; }
    Task<bool> MoveAsync(bool forward);
    // TODO: Task LoadStateAsync(Stream stream);
    // TODO: Task SaveStateAsync(Stream stream);
}
