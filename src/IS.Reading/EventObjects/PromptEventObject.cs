using System;

namespace IS.Reading.EventObjects
{
    public class PromptEventObject<T> : IPromptEvents<T>, IPromptEventCaller<T>
    {
        public event EventHandler<T>? OnOpen;

        void IPromptEventCaller<T>.Open(T prompt)
            => OnOpen?.Invoke(this, prompt);
    }
}
