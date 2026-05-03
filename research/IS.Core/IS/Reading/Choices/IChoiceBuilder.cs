using IS.Reading.Navigation;

namespace IS.Reading.Choices;

public interface IChoiceBuilder
{
    IChoice? Build(INavigationContext context);
}
