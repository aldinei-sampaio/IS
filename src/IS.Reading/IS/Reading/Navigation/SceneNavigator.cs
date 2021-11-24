namespace IS.Reading.Navigation;

public class SceneNavigator : ISceneNavigator
{
    private readonly IBlockNavigator blockNavigator;

    public SceneNavigator(IBlockNavigator blockNavigator)
        => this.blockNavigator = blockNavigator;

    public async Task<bool> MoveAsync(IStoryboard storyboard, INavigationContext context, bool forward)
    {
        if (storyboard.CurrentBlock is null)
            storyboard.CurrentBlock = storyboard.RootBlock;

        var block = storyboard.CurrentBlock;

        for (; ; )
        {
            var item = await blockNavigator.MoveAsync(block, context, forward);

            if (item is null)
            {
                if (!storyboard.EnteredBlocks.TryPop(out var parentBlock))
                {
                    storyboard.CurrentNode = null;
                    storyboard.CurrentBlock = null;
                    return false;
                }

                block = parentBlock;
                storyboard.CurrentBlock = parentBlock;
                continue;
            }

            if (item.ChildBlock is not null)
            {
                block = item.ChildBlock;
                storyboard.EnteredBlocks.Push(storyboard.CurrentBlock);
                storyboard.CurrentBlock = block;
                continue;
            }

            if (item is IPauseNode)
            {
                storyboard.CurrentNode = item;
                return true;
            }
        }
    }
}
