using IS.Reading.Conditions;

namespace IS.Reading.Parsing.AttributeParsers
{
    public class WhenAttribute : IAttribute
    {
        public ICondition Condition { get; }
        public WhenAttribute(ICondition condition)
            => Condition = condition;
    }
}
