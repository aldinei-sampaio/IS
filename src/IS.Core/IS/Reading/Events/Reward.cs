namespace IS.Reading.Events;

public class Reward
{
    public string Text { get; }

    public Reward(string text)
    {
        Text = text;
    }

    public override string ToString()
    {
        return Text;
    }
}
