using IS.Reading.Navigation;

namespace IS.Reading.Choices;

public class BuilderPropertySetter<T> : IBuilder<T>
{
    public BuilderPropertySetter(Action<T> buildAction)
        => BuildAction = buildAction;

    public Action<T> BuildAction { get; }

    public void Build(T prototype, INavigationContext context)
        => BuildAction(prototype);
}
