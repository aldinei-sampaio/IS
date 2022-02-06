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
            if (settings.ExitOnElse)
            {
                if (string.Compare(reader.ElementName, "else", true) == 0)
                    return;
            }

            if (!TryGetParser(reader, parsingContext, settings, processed, out var parser))
            {
                if (settings.ExitOnUnknownNode)
                    return;

                if (settings.IsBlock && string.Compare(reader.ElementName, "end", true) == 0)
                    return;

                parsingContext.LogError(reader, $"Elemento não reconhecido: '{reader.ElementName}'");
                return;
            }
            else
            {
                if (parser.IsArgumentRequired && string.IsNullOrEmpty(reader.Argument))
                {
                    parsingContext.LogError(reader, $"O comando '{reader.ElementName}' requer um argumento.");
                    return;
                }

                if (!await ParseElementAsync(reader, parsingContext, parentParsingContext, settings, parser))
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
        IElementParserSettings settings,
        INodeParser parser
    )
    {
        if (parser is IAggregateNodeParser)
        {
            await parser.ParseAsync(reader, parsingContext, parentParsingContext);
            return false;
        }

        if (settings.IsBlock)
        {
            using var childReader = reader.ReadSubtree();
            await parser.ParseAsync(childReader, parsingContext, parentParsingContext);
        }
        else
        {
            await parser.ParseAsync(reader, parsingContext, parentParsingContext);
        }

        return true;
    }
}
