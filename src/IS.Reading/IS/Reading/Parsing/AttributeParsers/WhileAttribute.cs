using IS.Reading.Conditions;

namespace IS.Reading.Parsing.AttributeParsers
{
    public class WhileAttribute : IAttribute
    {
        public ICondition Condition { get; }
        public WhileAttribute(ICondition condition)
            => Condition = condition;
    }
}
