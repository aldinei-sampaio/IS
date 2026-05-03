using Microsoft.Extensions.DependencyInjection;

namespace IS.Reading.Parsing.NodeParsers;

public class ElementParserSettingsFactory : IElementParserSettingsFactory
{
    private readonly IServiceProvider serviceProvider;
    private readonly Lazy<IElementParserSettings> ifBlock;
    private readonly Lazy<IElementParserSettings> block;
    private readonly Lazy<IElementParserSettings> noBlock;

    public ElementParserSettingsFactory(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
        ifBlock = new(() => new ElementParserSettings.IfBlock(GetNodeParsers()));
        block = new(() => new ElementParserSettings.Block(GetNodeParsers()));
        noBlock = new(() => new ElementParserSettings.NoBlock(GetNodeParsers()));
    }

    public IElementParserSettings IfBlock => ifBlock.Value;
    public IElementParserSettings Block => block.Value;
    public IElementParserSettings NoBlock => noBlock.Value;

    private INodeParser[] GetNodeParsers()
    {
        return new INodeParser[]
        {
            serviceProvider.GetRequiredService<IMusicNodeParser>(),
            serviceProvider.GetRequiredService<IBackgroundNodeParser>(),
            serviceProvider.GetRequiredService<IPauseNodeParser>(),
            serviceProvider.GetRequiredService<IMainCharacterNodeParser>(),
            serviceProvider.GetRequiredService<IPersonNodeParser>(),
            serviceProvider.GetRequiredService<INarrationNodeParser>(),
            serviceProvider.GetRequiredService<ITutorialNodeParser>(),
            serviceProvider.GetRequiredService<ISetNodeParser>(),
            serviceProvider.GetRequiredService<IInputNodeParser>(),
            serviceProvider.GetRequiredService<IIfNodeParser>(),
            serviceProvider.GetRequiredService<IWhileNodeParser>()
        };
    }
}
