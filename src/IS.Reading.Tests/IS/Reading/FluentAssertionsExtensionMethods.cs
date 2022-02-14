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
}

//internal class ObjectAssertions<T> : ObjectAssertions<T, ObjectAssertions<T>>
//{
//    public ObjectAssertions(T value) : base(value)
//    { 
//    }

//    public AndConstraint<ObjectAssertions<T>> Satisfy(Action<T> predicate)
//    {
//        predicate.Invoke(this.Subject);
//        return new AndConstraint<ObjectAssertions<T>>(this);
//    }
//}