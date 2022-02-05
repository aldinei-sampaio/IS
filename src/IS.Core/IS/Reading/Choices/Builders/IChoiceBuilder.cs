using IS.Reading.Variables;

namespace IS.Reading.Choices.Builders;

public interface IChoiceBuilder
{
    IChoice? Build(IVariableDictionary variables);
}
