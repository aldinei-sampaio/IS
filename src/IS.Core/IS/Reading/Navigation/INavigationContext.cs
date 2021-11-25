using IS.Reading.Events;
using IS.Reading.State;

namespace IS.Reading.Navigation;

public interface INavigationContext
{
    IState State { get; }
    IVariableDictionary Variables { get; }
    IEventInvoker Events { get; }
}
