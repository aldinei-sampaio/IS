using IS.Reading.Parsing.AttributeParsers;
using System.Xml;

namespace IS.Reading.Parsing;

public class ElementParser : IElementParser
{
    public async Task ParseAsync(
        XmlReader reader, 
        IParsingContext parsingContext,
        IParentParsingContext parentParsingContext,
        IElementParserSettings settings
    )
    {
        if (reader.ReadState == ReadState.Initial)
        {
            await reader.MoveToContentAsync();

            if (reader.HasAttributes)
                ParseAttributes(reader, parsingContext, parentParsingContext, settings);

            if (!await reader.ReadAsync())
                return;
        }

        var textFound = false;
        var elementFound = false;

        HashSet<INodeParser>? processed = null;
        if (settings.NoRepeatNode)
            processed = new();

        for(; ; )
        {
            switch(reader.NodeType)
            {
                case XmlNodeType.Element:
                    elementFound = true;

                    if (!settings.ChildParsers.TryGet(reader.LocalName, out var parser))
                    {
                        if (settings.ExitOnUnknownNode)
                            return;

                        parsingContext.LogError(reader, $"Elemento não reconhecido: {reader.LocalName}");
                        break;
                    }

                    if (settings.NoRepeatNode && !processed!.Add(parser))
                    {
                        if (settings.ExitOnUnknownNode)
                            return;

                        parsingContext.LogError(reader, $"Elemento repetido: {reader.LocalName}");
                        break;
                    }

                    if (await ParseElementAsync(reader, parsingContext, parentParsingContext, parser, textFound))
                    {
                        if (reader.ReadState == ReadState.EndOfFile)
                            return;
                        continue;
                    }

                    break;

                case XmlNodeType.Text:
                    if (settings.TextParser is null && settings.ExitOnUnknownNode)
                        return;

                    textFound = true;
                    ParseText(reader, parsingContext, parentParsingContext, settings, elementFound);
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

            if (!await reader.ReadAsync())
                return;
        }
    }

    private static void ParseText(
        XmlReader reader, 
        IParsingContext parsingContext, 
        IParentParsingContext parentParsingContext,
        IElementParserSettings settings, 
        bool elementFound
    )
    {
        if (elementFound)
        {
            parsingContext.LogError(reader, "Não é permitido texto dentro de elemento que tenha elementos filhos.");
            return;
        }

        if (settings.TextParser is null)
        {
            parsingContext.LogError(reader, "Este elemento não permite texto.");
            return;
        }

        var parsedText = settings.TextParser.Parse(reader, parsingContext, reader.Value);
        if (parsedText is not null)
            parentParsingContext.ParsedText = parsedText;
    }

    private static async Task<bool> ParseElementAsync(
        XmlReader reader, 
        IParsingContext parsingContext,
        IParentParsingContext parentParsingContext,
        INodeParser parser,
        bool textFound
    )
    {
        if (textFound)
        {
            parsingContext.LogError(reader, "Não é permitido texto dentro de elemento que tenha elementos filhos.");
            return false;
        }

        if (parser is IAggregateNodeParser)
        {
            await parser.ParseAsync(reader, parsingContext, parentParsingContext);
            return true;
        }

        using var childReader = reader.ReadSubtree();
        await parser.ParseAsync(childReader, parsingContext, parentParsingContext);

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
