using IS.Reading.Conditions;
using IS.Reading.Navigation;

namespace IS.Reading.Nodes
{
    public class PauseNode : IPauseNode
    {
        public PauseNode(ICondition? when)
            => When = when;

        public ICondition? When { get; }
    }
}
