﻿namespace IS.Reading.Variables;

public interface IInterpolatedValue
{
    string Value { get; }
    bool IsVariable { get; }

    string ToString(IVariableDictionary variables);
}
