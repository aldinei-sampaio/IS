namespace IS.Reading.Parsing;

public class ElementParser : IElementParser
{
    private static bool IsElse(string command)
        => string.Compare(command, "else", true) == 0 || string.Compare(command, "elseif", true) == 0;

    private static bool IsEnd(string command)
        => string.Compare(command, "end", true) == 0;

    public async Task ParseAsync(
        IDocumentReader reader, 
        IParsingContext parsingContext,
        IParentParsingContext parentParsingContext,
        IElementParserSettings settings
    )
    {
        if (!settings.ParseCurrent && !await reader.ReadAsync())
            return;

        var processed = settings.NoRepeatNode ? new HashSet<INodeParser>() : null;

        do
        {
            if (settings.ExitOnElse && IsElse(reader.Command))
                return;

            if (settings.ExitOnEnd && IsEnd(reader.Command))
                return;

            var parser = GetParser(reader, parsingContext, settings, processed);
            if (parser is null)
                return;

            await parser.ParseAsync(reader, parsingContext, parentParsingContext);

            if (parser is not IAggregateNodeParser)
                await reader.ReadAsync();
        }
        while (!reader.AtEnd);
    }

    private static INodeParser? GetParser(
        IDocumentReader reader,
        IParsingContext parsingContext,
        IElementParserSettings settings,
        HashSet<INodeParser>? processed
    )
    {
        var parser = settings.ChildParsers[reader.Command];
        
        if (parser is null || (processed is not null && !processed.Add(parser)))
        {
            if (!settings.ExitOnUnknownNode)
                parsingContext.LogError(reader, $"Comando não reconhecido: '{reader.Command}'.");

            return null;
        }

        if (parser.IsArgumentRequired && string.IsNullOrEmpty(reader.Argument))
        {
            parsingContext.LogError(reader, $"O comando '{reader.Command}' requer um argumento.");
            return null;
        }

        return parser;
    }
}
