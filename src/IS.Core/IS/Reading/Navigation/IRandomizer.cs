namespace IS.Reading.Navigation;

public interface IRandomizer
{
    List<T> Shuffle<T>(IEnumerable<T> list);
}
