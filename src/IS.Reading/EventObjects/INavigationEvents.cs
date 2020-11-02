using System;

namespace IS.Reading.EventObjects
{
    public interface INavigationEvents
    {
        event EventHandler? OnMoveNext;
        event EventHandler? OnMovePrevious;
    }
}
