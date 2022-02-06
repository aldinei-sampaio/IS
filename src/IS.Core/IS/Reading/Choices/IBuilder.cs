using IS.Reading.Navigation;

namespace IS.Reading.Choices;

public interface IBuilder<T>
{
    void Build(T prototype, INavigationContext context);
}
