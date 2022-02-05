using IS.Reading.Variables;

namespace IS.Reading.Choices.Builders;

public class BuilderPropertySetter<T> : IBuilder<T>
{
    public BuilderPropertySetter(Action<T> buildAction)
        => BuildAction = buildAction;

    public Action<T> BuildAction { get; }

    public void Build(T prototype, IVariableDictionary variables)
        => BuildAction(prototype);
}
