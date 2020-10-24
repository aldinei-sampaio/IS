using System;

namespace IS.Reading.EventObjects
{
    public interface ISimpleEvents
    {
        event EventHandler<string>? OnChange;
    }
}
