namespace IS.App.Models;

public class BookModel
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string[] Categories { get; set; } = [];
}
