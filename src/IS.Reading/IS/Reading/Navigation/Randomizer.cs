using System.Diagnostics.CodeAnalysis;

namespace IS.Reading.Navigation;

public class Randomizer : IRandomizer
{
    [ThreadStatic] private static Random? local;

    public static void Seed(int seed)
        => local = new Random(seed);

    [ExcludeFromCodeCoverage]
    private static Random GetRandom()
        => local ??= new Random(unchecked(Environment.TickCount * 31 + Environment.CurrentManagedThreadId));

    public List<T> Shuffle<T>(List<T> source)
    {
        var random = GetRandom();
        return source.OrderBy(i => random.NextDouble()).ToList();
    }
}
