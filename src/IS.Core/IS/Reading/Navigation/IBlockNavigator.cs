namespace IS.Reading.Navigation;

public interface IBlockNavigator
{
    Task<INode?> MoveAsync(IBlock block, INavigationContext context, bool forward);
}
