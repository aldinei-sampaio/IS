﻿namespace IS.Reading
{
    public interface ICondition
    {
        bool Evaluate(IVariableDictionary variables);
    }
}
