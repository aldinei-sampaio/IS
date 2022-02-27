namespace IS.Reading.State;

public class NavigationState : INavigationState
{
    public IBackgroundState Background { get; set; } = BackgroundState.Empty;
    public string? MainCharacterName { get; set; }
    public string? PersonName { get; set; }
    public MoodType? MoodType { get; set; }
    public string? MusicName { get; set; }
    public int? CurrentBlockId { get; set; }
    public int CurrentIteration { get; set; }
    public IBlockStateDictionary BlockStates { get; }
    public string? WaitingFor { get; set; }
    public string? Title { get; set; }

    public NavigationState(IBlockStateDictionary blockStates)
        => BlockStates = blockStates;
}
