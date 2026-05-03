using IS.Reading.Navigation;
using IS.Reading.Parsing.NodeParsers;

namespace IS.Reading.Parsing;

public class RootBlockParser : IRootBlockParser
{
    public IElementParser ElementParser { get; }
    public IElementParserSettingsFactory ElementParserSettingsFactory { get; }

    public RootBlockParser(
        IElementParser elementParser,
        IElementParserSettingsFactory elementParserSettingsFactory
    )
    {
        ElementParser = elementParser;
        ElementParserSettingsFactory = elementParserSettingsFactory;
    }

    public async Task<List<INode>> ParseAsync(IDocumentReader reader, IParsingContext parsingContext)
    {
        var context = new ParentParsingContext();

        await ElementParser.ParseAsync(reader, parsingContext, context, ElementParserSettingsFactory.NoBlock);

        if (context.Nodes.Count == 0)
            parsingContext.LogError(reader, "Elemento filho era esperado.");

        return context.Nodes;
    }
}
