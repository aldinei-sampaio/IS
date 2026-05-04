namespace IS.Reading.Events;

public class MusicChangeEvent(string? musicName) : IMusicChangeEvent
{
    public string? MusicName { get; } = musicName;

    public override string ToString()
    {
        if (MusicName is null)
            return "music undefined";
        else
            return $"music: {MusicName}";
    }
}
