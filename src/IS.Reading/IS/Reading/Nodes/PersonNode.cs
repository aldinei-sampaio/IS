using IS.Reading.Events;
using IS.Reading.Navigation;

namespace IS.Reading.Nodes
{
    public class PersonNode : INode
    {
        public string PersonName { get; }

        public PersonNode(string personName, IBlock childBlock)
            => (PersonName, ChildBlock) = (personName, childBlock);
        
        public IBlock? ChildBlock { get; }

        private static bool IsProtagonist(INavigationContext context, string name)
            => name == context.State.ProtagonistName;

        public async Task<INode> EnterAsync(INavigationContext context)
        {
            var @event = new PersonEnterEvent(PersonName, IsProtagonist(context, PersonName));
            await context.Events.InvokeAsync<IPersonEnterEvent>(@event);
            context.State.PersonName = PersonName;
            context.State.MoodType = null;
            return this;
        }

        public async Task LeaveAsync(INavigationContext context)
        {
            var @event = new PersonLeaveEvent(PersonName, IsProtagonist(context, PersonName));
            await context.Events.InvokeAsync<IPersonLeaveEvent>(@event);
            context.State.MoodType = null;
            context.State.PersonName = null;
            return;
        }
    }
}
