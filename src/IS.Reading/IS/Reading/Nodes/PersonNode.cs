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

        public static async Task<object?> ApplyStateAsync(INavigationContext context, string personName)
        {
            var @event = new PersonEnterEvent(personName, IsProtagonist(context, personName));
            await context.Events.InvokeAsync<IPersonEnterEvent>(@event);
            context.State.PersonName = personName;
            context.State.MoodType = null;
            return null;
        }

        public Task<object?> EnterAsync(INavigationContext context)
            => ApplyStateAsync(context, PersonName);

        public async Task EnterAsync(INavigationContext context, object? state)
        {
            if (state is string personName)
                await ApplyStateAsync(context, personName);
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
