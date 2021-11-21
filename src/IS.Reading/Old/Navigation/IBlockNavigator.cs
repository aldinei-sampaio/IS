
namespace IS.Reading.Navigation
{
    public interface IBlockNavigator
    {
        Task<INode?> MoveNextAsync(IBlock block, IContext context);
        Task<INode?> MovePreviousAsync(IBlock block, IContext context);
    }
}