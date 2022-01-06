namespace IS.Reading.Parsing.ConditionParsers;

public interface IConditionParser
{
    IParsedCondition Parse(string text);
}
