﻿using System;

namespace IS.Reading.EventObjects
{
    public interface IPersonEvents
    {
        event EventHandler<string>? OnArrive;
        event EventHandler? OnLeave;

        ISimpleEvents Mood { get; }
        IOpenCloseEvents Thought { get; }
        IOpenCloseEvents Speech { get; }
        IPromptEvents<string> Reward { get; }
    }
}
