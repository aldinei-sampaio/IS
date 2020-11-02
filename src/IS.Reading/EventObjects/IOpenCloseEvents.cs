using System;

namespace IS.Reading.EventObjects
{
    public interface IOpenCloseEvents : ISimpleEvents
    {
        public event EventHandler? OnOpen;
        public event EventHandler? OnClose;
    }
}
