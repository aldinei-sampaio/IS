namespace IS.Reading.Navigation
{
    public interface INavigationBlockNode : INavigationNode
    {
        INavigationBlock Block { get; }
    }
}
