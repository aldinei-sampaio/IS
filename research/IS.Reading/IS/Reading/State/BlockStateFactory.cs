using Microsoft.Extensions.DependencyInjection;

namespace IS.Reading.State;

public class BlockStateFactory : IBlockStateFactory
{
    private readonly IServiceProvider serviceProvider;

    public BlockStateFactory(IServiceProvider serviceProvider)
        => this.serviceProvider = serviceProvider;

    public IBlockIterationState CreateIterationState() 
        => serviceProvider.GetRequiredService<IBlockIterationState>();

    public IBlockState CreateState()
        => serviceProvider.GetRequiredService<IBlockState>();

    public IBlockStateDictionary CreateStateDictionary()
        => serviceProvider.GetRequiredService<IBlockStateDictionary>();
}
