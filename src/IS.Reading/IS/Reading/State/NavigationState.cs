namespace IS.Reading.State;

public class NavigationState : INavigationState
{
    public IBackgroundState Background { get; set; } = BackgroundState.Empty;
}
