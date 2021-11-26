namespace IS.Reading.State;

public class NavigationState : INavigationState
{
    public IBackgroundState Background { get; set; } 
        = new BackgroundState(string.Empty, BackgroundType.Undefined, BackgroundPosition.Undefined);
}
