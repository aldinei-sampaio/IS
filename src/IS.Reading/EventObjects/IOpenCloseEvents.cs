using System;

namespace IS.Reading.EventObjects
{
    public interface IOpenCloseEvents
    {
        public event EventHandler? OnOpen;
        public event EventHandler? OnClose;
    }
}
