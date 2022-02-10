namespace IS.Reading.Navigation;

public interface IRandomizer
{
    IEnumerable<T> Shuffle<T>(IEnumerable<T> list);
}
