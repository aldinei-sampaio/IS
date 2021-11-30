namespace IS.Reading.State;

public interface INavigationState
{
    IBackgroundState Background { get; set; }
    string? Protagonist { get; set; }
    string? PersonName { get; set; }
    MoodType? MoodType { get; set; }
}