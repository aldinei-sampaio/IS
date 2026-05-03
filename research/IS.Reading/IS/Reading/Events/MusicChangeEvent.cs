namespace IS.Reading.Events;

public class MusicChangeEvent : IMusicChangeEvent
{
    public MusicChangeEvent(string? musicName)
        => MusicName = musicName;

    public string? MusicName { get; }

    public override string ToString()
    {
        if (MusicName is null)
            return "music undefined";
        else
            return $"music: {MusicName}";
    }        
}
