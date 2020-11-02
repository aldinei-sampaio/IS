using System;

namespace IS.Reading.EventObjects
{

    public class OpenCloseEventObject : SimpleEventObject, IOpenCloseEvents, IOpenCloseEventCaller
    {
        public event EventHandler? OnOpen;
        public event EventHandler? OnClose;

        void IOpenCloseEventCaller.Open()
            => OnOpen?.Invoke(this, EventArgs.Empty);

        void IOpenCloseEventCaller.Close()
            => OnClose?.Invoke(this, EventArgs.Empty);
    }
}
