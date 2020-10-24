using System;

namespace IS.Reading.EventObjects
{
    public class SimpleEventObject : ISimpleEvents, ISimpleEventCaller
    {
        public event EventHandler<string>? OnChange;

        void ISimpleEventCaller.Change(string value) 
            => OnChange?.Invoke(this, value);
    }
}
