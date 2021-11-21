﻿namespace IS.Reading.EventObjects;

public class SimpleEventObject : ISimpleEvents, ISimpleEventCaller
{
    public event AsyncEventHandler<EventArgs<string>>? OnChangeAsync;

    Task ISimpleEventCaller.ChangeAsync(string value)
        => OnChangeAsync.InvokeAllAsync(this, new(value));
}