namespace IS.Reading.Events;

public class Display
{
    public string ImageName { get; }
    public string Caption { get; }
    public Display(string imageName, string caption)
        => (ImageName, Caption) = (imageName, caption);

    public override string ToString() => $"({ImageName}) {Caption}";
}
