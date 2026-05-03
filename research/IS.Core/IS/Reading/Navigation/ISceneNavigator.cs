namespace IS.Reading.Navigation;

public interface ISceneNavigator
{
    Task<bool> MoveAsync(INavigationContext context, bool forward);
}
