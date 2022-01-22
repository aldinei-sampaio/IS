using IS.Reading.State;

namespace IS.Reading.Navigation;

public class SceneNavigator : ISceneNavigator
{
    private readonly IBlockNavigator blockNavigator;

    public SceneNavigator(IBlockNavigator blockNavigator)
        => this.blockNavigator = blockNavigator;

    public async Task<bool> MoveAsync(INavigationContext context, bool forward)
    {
        if (context.CurrentBlock is null)
        {
            context.CurrentBlock = context.RootBlock;
            context.CurrentBlockState = context.RootBlockState;
        }

        var block = context.CurrentBlock;
        var blockState = context.CurrentBlockState;

        for (; ; )
        {
            var item = await blockNavigator.MoveAsync(block, blockState, context, forward);

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
                blockState = context.EnteredBlockStates.Pop();
                context.CurrentBlockState = blockState;
                continue;
            }

            if (item.ChildBlock is not null)
            {
                block = item.ChildBlock;
                context.EnteredBlocks.Push(context.CurrentBlock);
                context.CurrentBlock = block;

                context.EnteredBlockStates.Push(blockState);
                blockState = blockState.GetCurrentIteration().Children[block.Id];
                context.CurrentBlockState = blockState;

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
