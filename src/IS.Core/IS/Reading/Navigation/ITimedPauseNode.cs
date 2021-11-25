namespace IS.Reading.Navigation;

public interface ITimedPauseNode : INode
{
    TimeSpan Duration { get; }
}
