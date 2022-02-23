using FluentAssertions.Collections;
using FluentAssertions.Execution;
using System.Runtime.CompilerServices;

namespace IS;

internal static class FluentAssertionsExtensionMethods
{
    public static void ShouldSatisfy<T>(
        this T subject, 
        Action<T> predicate, 
        [CallerArgumentExpression("subject")] string context = null
    )
    {
        using (new AssertionScope(context))
            predicate.Invoke(subject);
    }

    public static AndConstraint<GenericCollectionAssertions<T>> BeEquivalentTo<T>(
        this GenericCollectionAssertions<T> assertions,
        params T[] values
    )
    {
        return assertions.BeEquivalentTo(values);
    }

    public static AndConstraint<GenericCollectionAssertions<T>> Contain<T>(
        this GenericCollectionAssertions<T> assertions,
        params Action<T>[] validations
    )
    {
        using var e1 = assertions.Subject.GetEnumerator();
        using var e2 = ((IEnumerable<Action<T>>)validations).GetEnumerator();

        var count = 0;
        for(; ; )
        {
            if (!e1.MoveNext())
            {
                if (!e2.MoveNext())
                    break;

                throw new Exception($"A collection possui apenas {count} elementos, mas eram esperados {validations.Length}.");
            }
            else if (!e2.MoveNext())
                throw new Exception($"A collection possui {assertions.Subject.Count()} elementos, mas eram esperados apenas {validations.Length}.");

            count++;
            using (new AssertionScope($"Item[{count}]"))
                e2.Current.Invoke(e1.Current);
        }

        return new AndConstraint<GenericCollectionAssertions<T>>(assertions);
    }
}