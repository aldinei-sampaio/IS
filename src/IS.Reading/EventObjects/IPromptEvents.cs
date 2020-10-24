using System;

namespace IS.Reading.EventObjects
{
    public interface IPromptEvents<T>
    {
        public event EventHandler<T>? OnOpen;
    }
}
