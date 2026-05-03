using IS.Reading.State;

namespace IS.Reading.Navigation;

public interface IBlockNavigator
{
    Task<INode?> MoveAsync(IBlock block, IBlockState blockState, INavigationContext context, bool forward);
}
