namespace IS.Reading.State;

public interface INavigationState
{
    IBackgroundState Background { get; set; }
    string? ProtagonistName { get; set; }
    string? PersonName { get; set; }
    MoodType? MoodType { get; set; }
}