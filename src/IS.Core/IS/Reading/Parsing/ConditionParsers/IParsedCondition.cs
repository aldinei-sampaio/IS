using IS.Reading.Conditions;

namespace IS.Reading.Parsing.ConditionParsers;

public interface IParsedCondition
{
    ICondition? Condition { get; }
    string Message { get; }
}
