using System;

namespace IS.Reading.EventObjects
{
    public class NavigationEventObject : INavigationEvents, INavigationEventCaller
    {
        public event EventHandler? OnMoveNext;
        public event EventHandler? OnMovePrevious;

        public void MoveNext() => OnMoveNext?.Invoke(this, EventArgs.Empty);
        public void MovePrevious() => OnMovePrevious?.Invoke(this, EventArgs.Empty);
    }
}
