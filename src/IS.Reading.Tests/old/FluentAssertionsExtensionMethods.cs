using FluentAssertions.Execution;

namespace IS;

internal static class FluentAssertionsExtensionMethods
{
    public static void ShouldSatisfy<T>(this T subject, Action<T> predicate)
    {
        using (new AssertionScope())
            predicate.Invoke(subject);
    }
}