using IS.Reading.Conditions;

namespace IS.Reading.Parsing.ConditionParsers;

public interface IConditionParser
{
    Result<ICondition> Parse(string value);
}
