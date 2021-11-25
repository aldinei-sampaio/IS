namespace IS.Reading.State;

public record BackgroundState(
    string Name,
    BackgroundType Type,
    BackgroundPosition Position
) : IBackgroundState
{
    public override string ToString()
    {
        if (Type == BackgroundType.Color)
            return "bg color: " + Name;

        if (Type == BackgroundType.Image)
        {
            if (Position == BackgroundPosition.Left)
                return "bg left: " + Name;
            
            if (Position == BackgroundPosition.Right)
                return "bg right: " + Name;

            return "bg: " + Name;
        }

        return "bg empty";
    }
}
