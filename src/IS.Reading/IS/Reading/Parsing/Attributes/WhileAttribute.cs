using IS.Reading.Navigation;

namespace IS.Reading.Parsing.Attributes
{
    public class WhileAttribute : IAttribute
    {
        public ICondition Condition { get; }
        public WhileAttribute(ICondition condition)
            => Condition = condition;
    }
}
