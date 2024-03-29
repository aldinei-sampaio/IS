﻿namespace IS.Reading.State;

public interface INavigationState
{
    IBackgroundState Background { get; set; }
    string? MainCharacterName { get; set; }
    string? PersonName { get; set; }
    MoodType? MoodType { get; set; }
    string? MusicName { get; set; }
    int? CurrentBlockId { get; set; }
    int CurrentIteration { get; set; }
    string? WaitingFor { get; set; }
    string? Title { get; set; }
}