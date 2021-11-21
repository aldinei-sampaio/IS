namespace IS.Reading.Navigation;

public interface ISceneNavigator
{
    Task<bool> MoveAsync(IStoryboard storyboard, IContext context, bool forward);
}
