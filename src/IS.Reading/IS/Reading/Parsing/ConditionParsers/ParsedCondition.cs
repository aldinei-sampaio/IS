using IS.Reading.Conditions;

namespace IS.Reading.Parsing.ConditionParsers;

public record ParsedCondition(ICondition? Condition, string Message) : IParsedCondition;
