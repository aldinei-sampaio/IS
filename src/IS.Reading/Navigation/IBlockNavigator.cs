
namespace IS.Reading.Navigation
{
    public interface IBlockNavigator
    {
        Task<INavigationNode?> MoveNextAsync(INavigationBlock block, INavigationContext context);
        Task<INavigationNode?> MovePreviousAsync(INavigationBlock block, INavigationContext context);
    }
}