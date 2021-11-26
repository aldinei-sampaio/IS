using IS.Reading.Events;

namespace IS.Reading.Navigation;

public interface IStoryboard : IDisposable
{
    IEventSubscriber Events { get; }
    Task LoadStateAsync(Stream stream);
    Task<bool> MoveAsync(bool forward);
    Task SaveStateAsync(Stream stream);
}
