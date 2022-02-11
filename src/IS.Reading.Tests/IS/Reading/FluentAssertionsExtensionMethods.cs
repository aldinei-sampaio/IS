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