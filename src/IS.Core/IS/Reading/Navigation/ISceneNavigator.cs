namespace IS.Reading.Navigation;

public interface ISceneNavigator
{
    Task<bool> MoveAsync(IStoryboard storyboard, INavigationContext context, bool forward);
}
