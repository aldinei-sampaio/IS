using IS.Reading.Conditions;
using IS.Reading.Events;
using IS.Reading.Navigation;
using IS.Reading.State;

namespace IS.Reading.Nodes
{
    public class ScrollNode : INode
    {
        public async Task<object?> EnterAsync(INavigationContext context)
        {
            var oldState = context.State.Background;

            if (oldState.Type != BackgroundType.Image || oldState.Position == BackgroundPosition.Undefined)
                return null;

            var newPosition = oldState.Position == BackgroundPosition.Right ? BackgroundPosition.Left : BackgroundPosition.Right;
            var newState = new BackgroundState(oldState.Name, oldState.Type, newPosition);

            await context.Events.InvokeAsync<IBackgroundScrollEvent>(new BackgroundScrollEvent(newPosition));
            context.State.Background = newState;

            return null;
        }
    }
}
