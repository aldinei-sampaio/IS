namespace IS.App.Models;

public class BookDetailsModel
{
    public string Title { get; set; } = string.Empty;
    public string Synopsis { get; set; } = string.Empty;
    public string[] Tags { get; set; } = [];
    public int ReleasedChapters { get; set; }
    public bool FullReleased { get; set; }
}
