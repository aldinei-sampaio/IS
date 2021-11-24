namespace IS.Reading.Events;

public class Trophy
{
    public string Code { get; }
    public string Title { get; }
    public string Description { get; }
    public string Requirement { get; }
    public Trophy(string code, string title, string description, string requirement)
    {
        Code = code;
        Title = title;
        Description = description;
        Requirement = requirement;
    }
}
