﻿namespace IS.Reading
{
    public interface ICondition
    {
        bool Evaluate(IStoryContextEventCaller context);
    }
}
