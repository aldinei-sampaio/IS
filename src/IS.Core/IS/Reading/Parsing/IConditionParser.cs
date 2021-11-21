using IS.Reading.Navigation;

namespace IS.Reading.Parsing;

public interface IConditionParser
{
    ICondition? Parse(string expression);
}
