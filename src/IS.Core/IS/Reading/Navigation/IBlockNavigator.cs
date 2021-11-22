namespace IS.Reading.Navigation;

public interface IBlockNavigator
{
    Task<INode?> MoveAsync(IBlock block, IContext context, bool forward);
}
