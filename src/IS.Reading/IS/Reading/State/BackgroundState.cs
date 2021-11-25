namespace IS.Reading.State;

public record BackgroundState(
    string Name,
    BackgroundType Type,
    BackgroundPosition Position
) : IBackgroundState;
