using IS.Reading.Events;
using IS.Reading.Navigation;

namespace IS.Reading.Nodes
{
    public class PersonNode : INode
    {
        public string Name { get; }

        public PersonNode(string name, IBlock childBlock)
            => (Name, ChildBlock) = (name, childBlock);
        
        public IBlock? ChildBlock { get; }

        private static bool IsProtagonist(INavigationContext context, string name)
            => name == context.State.Protagonist;

        public async Task<INode> EnterAsync(INavigationContext context)
        {
            var @event = new PersonEnterEvent(Name, IsProtagonist(context, Name));
            await context.Events.InvokeAsync<IPersonEnterEvent>(@event);
            context.State.Person = Name;
            return this;
        }

        public async Task<INode> LeaveAsync(INavigationContext context)
        {
            var @event = new PersonLeaveEvent(Name, IsProtagonist(context, Name));
            await context.Events.InvokeAsync<IPersonLeaveEvent>(@event);
            context.State.MoodType = null;
            context.State.Person = null;
            return this;
        }
    }
}
