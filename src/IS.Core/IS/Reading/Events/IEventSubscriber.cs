﻿namespace IS.Reading.Events;

public interface IEventSubscriber
{
    void Subscribe<T>(Func<T, Task> handler) where T : IReadingEvent;
}
