namespace IS.Reading.State;

public interface IState
{
    IBackgroundState Background { get; set; }
}