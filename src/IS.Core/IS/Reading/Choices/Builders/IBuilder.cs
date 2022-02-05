using IS.Reading.Variables;

namespace IS.Reading.Choices.Builders;

public interface IBuilder<T>
{
    void Build(T prototype, IVariableDictionary variables);
}
