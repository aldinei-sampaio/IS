using IS.Reading.Navigation;

namespace IS.Reading.Parsing.Attributes
{
    public class WhenAttribute : IAttribute
    {
        public ICondition Condition { get; }
        public WhenAttribute(ICondition condition)
            => Condition = condition;
    }
}
