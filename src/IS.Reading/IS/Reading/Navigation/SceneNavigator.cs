namespace IS.Reading.Navigation;

public class SceneNavigator : ISceneNavigator
{
    private readonly IBlockNavigator blockNavigator;

    public SceneNavigator(IBlockNavigator blockNavigator)
        => this.blockNavigator = blockNavigator;

    public async Task<bool> MoveAsync(INavigationContext context, bool forward)
    {
        if (context.CurrentBlock is null)
            context.CurrentBlock = context.RootBlock;

        var block = context.CurrentBlock;

        for (; ; )
        {
            var item = await blockNavigator.MoveAsync(block, context, forward);

            if (item is null)
            {
                if (!context.EnteredBlocks.TryPop(out var parentBlock))
                {
                    context.CurrentNode = null;
                    context.CurrentBlock = null;
                    return false;
                }

                block = parentBlock;
                context.CurrentBlock = parentBlock;
                continue;
            }

            if (item.ChildBlock is not null)
            {
                block = item.ChildBlock;
                context.EnteredBlocks.Push(context.CurrentBlock);
                context.CurrentBlock = block;
                continue;
            }

            if (item is IPauseNode)
            {
                context.CurrentNode = item;
                return true;
            }
        }
    }
}
