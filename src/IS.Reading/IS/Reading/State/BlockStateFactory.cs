using Microsoft.Extensions.DependencyInjection;

namespace IS.Reading.State;

public class BlockStateFactory : IBlockStateFactory
{
    private readonly IServiceProvider serviceProvider;

    public BlockStateFactory(IServiceProvider serviceProvider)
        => this.serviceProvider = serviceProvider;

    public IBlockIterationState Create() 
        => serviceProvider.GetRequiredService<IBlockIterationState>();
}
