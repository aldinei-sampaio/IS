namespace IS.Reading.State;

public class NavigationState : INavigationState
{
    public IBackgroundState Background { get; set; } = BackgroundState.Empty;
    public string? Protagonist { get; set; }
    public string? Person { get; set; }
    public MoodType? MoodType { get; set; }
}
