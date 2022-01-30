using System.Diagnostics.CodeAnalysis;

namespace IS.Reading.Parsing;

public class ElementParser : IElementParser
{
    public async Task ParseAsync(
        IDocumentReader reader, 
        IParsingContext parsingContext,
        IParentParsingContext parentParsingContext,
        IElementParserSettings settings
    )
    {
        if (!await reader.ReadAsync())
            return;

        var processed = settings.NoRepeatNode ? new HashSet<INodeParser>() : null;

        while (!reader.AtEnd)
        {
            if (!TryGetParser(reader, parsingContext, settings, processed, out var parser))
            {
                if (settings.ExitOnUnknownNode)
                    return;
            }
            else if (!await ParseElementAsync(reader, parsingContext, parentParsingContext, parser))
            {
                continue;
            }

            await reader.ReadAsync();
        }
    }

    private static bool TryGetParser(
        IDocumentReader reader,
        IParsingContext parsingContext,
        IElementParserSettings settings,
        HashSet<INodeParser>? processed,
        [MaybeNullWhen(false)] out INodeParser parser
    )
    {

        if (!settings.ChildParsers.TryGet(reader.ElementName, out parser))
        {
            if (!settings.ExitOnUnknownNode)
                parsingContext.LogError(reader, $"Elemento não reconhecido: {reader.ElementName}");

            return false;
        }

        if (processed is not null && !processed.Add(parser))
        {
            if (!settings.ExitOnUnknownNode)
                parsingContext.LogError(reader, $"Elemento repetido: {reader.ElementName}");
            
            return false;
        }

        return true;
    }

    private static async Task<bool> ParseElementAsync(
        IDocumentReader reader,
        IParsingContext parsingContext,
        IParentParsingContext parentParsingContext,
        INodeParser parser
    )
    {
        if (parser is IAggregateNodeParser)
        {
            await parser.ParseAsync(reader, parsingContext, parentParsingContext);
            return false;
        }

        using var childReader = reader.ReadSubtree();
        await parser.ParseAsync(childReader, parsingContext, parentParsingContext);

        return true;
    }
}
