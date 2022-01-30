using IS.Reading.Parsing.AttributeParsers;
using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace IS.Reading.Parsing;

public class ElementParserOld : IElementParser
{
    private record struct State(
        bool TextFound, 
        bool ElementFound, 
        HashSet<INodeParser>? Processed,
        IParsingContext ParsingContext,
        IParentParsingContext ParentParsingContext,
        IElementParserSettings Settings,
        XmlReader Reader
    );

    public async Task ParseAsync(
        XmlReader reader, 
        IParsingContext parsingContext,
        IParentParsingContext parentParsingContext,
        IElementParserSettings settings
    )
    {
        var state = new State(
            TextFound: false,
            ElementFound: false,
            Processed: null,
            parsingContext,
            parentParsingContext,
            settings,
            reader
        );

        if (!await InitializeAsync(state))
            return;

        state.Processed = settings.NoRepeatNode ? new() : null;

        while (reader.ReadState != ReadState.EndOfFile)
        {
            switch(reader.NodeType)
            {
                case XmlNodeType.Element:
                    state.ElementFound = true;

                    if (!TryGetParser(state, out var parser))
                    {
                        if (settings.ExitOnUnknownNode)
                            return;
                    }
                    else if (!await ParseElementAsync(state, parser))
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

    private static async Task<bool> InitializeAsync(State state)
    {
        if (state.Reader.ReadState != ReadState.Initial)
            return true;

        await state.Reader.MoveToContentAsync();

        if (state.Reader.HasAttributes)
            ParseAttributes(state);

        return await state.Reader.ReadAsync();
    }

    private static bool TryGetParser(State state, [MaybeNullWhen(false)] out INodeParser parser)
    {
        var reader = state.Reader;

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

    private static async Task<bool> ParseElementAsync(State state, INodeParser parser)
    {
        var reader = state.Reader;

        if (state.TextFound)
        {
            state.ParsingContext.LogError(reader, "Não é permitido texto dentro de elemento que tenha elementos filhos.");
            return true;
        }

        if (parser is IAggregateNodeParser)
        {
            await parser.ParseAsync(reader, state.ParsingContext, state.ParentParsingContext);
            return false;
        }

        using var childReader = reader.ReadSubtree();
        await parser.ParseAsync(childReader, state.ParsingContext, state.ParentParsingContext);

        return true;
    }

    private static void ParseAttributes(State state)
    {
        var reader = state.Reader;

        while (reader.MoveToNextAttribute())
        {
            if (!state.Settings.AttributeParsers.TryGet(reader.LocalName, out var attributeParser))
            {
                state.ParsingContext.LogError(reader, $"Atributo não reconhecido: {reader.LocalName}");
                continue;
            }

            var attribute = attributeParser.Parse(reader, state.ParsingContext);
            if (attribute is not null)
            {
                if (attribute is WhenAttribute whenAttribute)
                    state.ParentParsingContext.When = whenAttribute.Condition;

                if (attribute is WhileAttribute whileAttribute)
                    state.ParentParsingContext.While = whileAttribute.Condition;
            }
        }
        reader.MoveToElement();
    }
}
