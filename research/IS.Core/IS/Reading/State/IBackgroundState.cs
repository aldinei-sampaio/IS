namespace IS.Reading.State;

public interface IBackgroundState
{
    BackgroundType Type { get; }
    string Name { get; }
    BackgroundPosition Position { get; }
}
