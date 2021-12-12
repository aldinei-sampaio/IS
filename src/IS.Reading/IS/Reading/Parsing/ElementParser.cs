using IS.Reading.Parsing.AttributeParsers;
using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace IS.Reading.Parsing;

public class ElementParser : IElementParser
{
    private record struct State(
        bool TextFound, 
        bool ElementFound, 
        HashSet<INodeParser>? Processed,
        IParsingContext ParsingContext,
        IParentParsingContext ParentParsingContext,
        IElementParserSettings Settings
    );

    public async Task ParseAsync(
        XmlReader reader, 
        IParsingContext parsingContext,
        IParentParsingContext parentParsingContext,
        IElementParserSettings settings
    )
    {
        if (reader.ReadState == ReadState.Initial
            && !await InitializeAsync(reader, parsingContext, parentParsingContext, settings))
            return;

        var state = new State(
            TextFound: false,
            ElementFound: false,
            Processed: settings.NoRepeatNode ? new() : null,
            parsingContext,
            parentParsingContext,
            settings
        );

        while(reader.ReadState != ReadState.EndOfFile)
        {
            switch(reader.NodeType)
            {
                case XmlNodeType.Element:
                    state.ElementFound = true;

                    if (!TryGetParser(reader, state, out var parser))
                    {
                        if (settings.ExitOnUnknownNode)
                            return;
                    }
                    else if (await ParseElementAsync(reader, state, parser))
                        continue;

                    break;

                case XmlNodeType.Text:
                    if (settings.TextParser is null && settings.ExitOnUnknownNode)
                        return;

                    state.TextFound = true;
                    ParseText(reader, state);
                    break;

                case XmlNodeType.Whitespace:
                case XmlNodeType.SignificantWhitespace:
                case XmlNodeType.EndElement:
                case XmlNodeType.Comment:
                    // Ignore
                    break;

                default:
                    parsingContext.LogError(reader, $"Conteúdo inválido detectado: {reader.NodeType}");
                    break;

            }

            await reader.ReadAsync();
        }
    }

    private static async Task<bool> InitializeAsync(XmlReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext, IElementParserSettings settings)
    {
        await reader.MoveToContentAsync();

        if (reader.HasAttributes)
            ParseAttributes(reader, parsingContext, parentParsingContext, settings);

        return await reader.ReadAsync();
    }

    private static bool TryGetParser(XmlReader reader, State state, [MaybeNullWhen(false)] out INodeParser parser)
    {
        if (!state.Settings.ChildParsers.TryGet(reader.LocalName, out parser))
        {
            if (!state.Settings.ExitOnUnknownNode)
                state.ParsingContext.LogError(reader, $"Elemento não reconhecido: {reader.LocalName}");

            return false;
        }

        if (state.Processed is not null && !state.Processed.Add(parser))
        {
            if (!state.Settings.ExitOnUnknownNode)
                state.ParsingContext.LogError(reader, $"Elemento repetido: {reader.LocalName}");
            
            return false;
        }

        return true;
    }

    private static void ParseText(XmlReader reader, State state)
    {
        if (state.ElementFound)
        {
            state.ParsingContext.LogError(reader, "Não é permitido texto dentro de elemento que tenha elementos filhos.");
            return;
        }

        if (state.Settings.TextParser is null)
        {
            state.ParsingContext.LogError(reader, "Este elemento não permite texto.");
            return;
        }

        var parsedText = state.Settings.TextParser.Parse(reader, state.ParsingContext, reader.Value);
        if (parsedText is not null)
            state.ParentParsingContext.ParsedText = parsedText;
    }

    private static async Task<bool> ParseElementAsync(
        XmlReader reader, 
        State state,
        INodeParser parser
    )
    {
        if (state.TextFound)
        {
            state.ParsingContext.LogError(reader, "Não é permitido texto dentro de elemento que tenha elementos filhos.");
            return false;
        }

        if (parser is IAggregateNodeParser)
        {
            await parser.ParseAsync(reader, state.ParsingContext, state.ParentParsingContext);
            return true;
        }

        using var childReader = reader.ReadSubtree();
        await parser.ParseAsync(childReader, state.ParsingContext, state.ParentParsingContext);

        return false;
    }

    private static void ParseAttributes(
        XmlReader reader, 
        IParsingContext parsingContext, 
        IParentParsingContext parentParsingContext,
        IElementParserSettings settings
    )
    {
        while (reader.MoveToNextAttribute())
        {
            if (!settings.AttributeParsers.TryGet(reader.LocalName, out var attributeParser))
            {
                parsingContext.LogError(reader, $"Atributo não reconhecido: {reader.LocalName}");
                continue;
            }

            var attribute = attributeParser.Parse(reader, parsingContext);
            if (attribute is not null)
            {
                if (attribute is WhenAttribute whenAttribute)
                    parentParsingContext.When = whenAttribute.Condition;

                if (attribute is WhileAttribute whileAttribute)
                    parentParsingContext.While = whileAttribute.Condition;
            }
        }
        reader.MoveToElement();
    }
}
