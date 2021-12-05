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
        else
        {
            if (reader.HasAttributes)
                ParseAttributes(reader, parsingContext, parentParsingContext, settings);
        }

        var textFound = false;
        var elementFound = false;

        do
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

                    await ParseElementAsync(reader, parsingContext, parentParsingContext, parser, textFound);

                    break;

                case XmlNodeType.Text:
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
        }
        while (await reader.ReadAsync());
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

    private static async Task ParseElementAsync(
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
            return;
        }

        using var childReader = reader.ReadSubtree();
        await parser.ParseAsync(childReader, parsingContext, parentParsingContext);
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
