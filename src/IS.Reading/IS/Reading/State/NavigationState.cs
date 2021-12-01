namespace IS.Reading.State;

public class NavigationState : INavigationState
{
    public IBackgroundState Background { get; set; } = BackgroundState.Empty;
    public string? ProtagonistName { get; set; }
    public string? PersonName { get; set; }
    public MoodType? MoodType { get; set; }
}
