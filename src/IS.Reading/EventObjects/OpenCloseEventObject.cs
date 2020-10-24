using System;

namespace IS.Reading.EventObjects
{

    public class OpenCloseEventObject : SimpleEventObject, IOpenCloseEvents, IOpenCloseEventCaller
    {
        public event EventHandler? OnOpen;
        public event EventHandler? OnClose;

        void IOpenCloseEventCaller.Close()
            => OnOpen?.Invoke(this, EventArgs.Empty);

        void IOpenCloseEventCaller.Open()
            => OnClose?.Invoke(this, EventArgs.Empty);
    }
}
