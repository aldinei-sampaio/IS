using IS.Reading.Navigation;
using IS.Reading.Parsing.AttributeParsers;
using System.Xml;

namespace IS.Reading.Parsing;

public class ElementParser : IElementParser
{
    public async Task<IElementParsedData> ParseAsync(XmlReader reader, IParsingContext parsingContext, IElementParserSettings settings)
    {
        var parsed = new ElementParsedData();

        if (reader.HasAttributes)
            ParseAttributes(reader, parsingContext, settings, parsed);

        var textFound = false;
        var elementFound = false;

        while (await reader.ReadAsync())
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                elementFound = true;
                await ParseElementAsync(reader, parsingContext, settings, parsed, textFound);
            }
            else if (reader.NodeType == XmlNodeType.Text)
            {
                textFound = true;
                ParseText(reader, parsingContext, settings, parsed, elementFound);
            }
            else if (reader.NodeType != XmlNodeType.EndElement)
            {
                parsingContext.LogError(reader, $"Conteúdo inválido detectado: {reader.NodeType}");
            }
        }

        return parsed;
    }

    private static void ParseText(
        XmlReader reader, 
        IParsingContext parsingContext, 
        IElementParserSettings settings, 
        ElementParsedData parsed, 
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
            parsed.Text = parsedText;
    }

    private static async Task ParseElementAsync(
        XmlReader reader, 
        IParsingContext parsingContext,
        IElementParserSettings settings,
        ElementParsedData parsed, 
        bool textFound
    )
    {
        if (textFound)
        {
            parsingContext.LogError(reader, "Não é permitido texto dentro de elemento que tenha elementos filhos.");
            return;
        }

        if (!settings.ChildParsers.TryGetValue(reader.LocalName, out var parser))
        {
            parsingContext.LogError(reader, $"Elemento não reconhecido: {reader.LocalName}");
            return;
        }

        var childReader = reader.ReadSubtree();
        var child = await parser.ParseAsync(childReader, parsingContext);
        if (child is not null)
        {
            if (parsed.Block is null)
                parsed.Block = new Block();
            parsed.Block.ForwardQueue.Enqueue(child);
        }
    }

    private static void ParseAttributes(
        XmlReader reader, 
        IParsingContext parsingContext, 
        IElementParserSettings settings, 
        ElementParsedData parsed
    )
    {
        while (reader.MoveToNextAttribute())
        {
            if (!settings.AttributeParsers.TryGetValue(reader.LocalName, out var attributeParser))
            {
                parsingContext.LogError(reader, $"Atributo não reconhecido: {reader.LocalName}");
                continue;
            }

            var attribute = attributeParser.Parse(reader, parsingContext);
            if (attribute is not null)
            {
                if (attribute is WhenAttribute whenAttribute)
                    parsed.When = whenAttribute.Condition;

                if (attribute is WhileAttribute whileAttribute)
                    parsed.While = whileAttribute.Condition;
            }
        }
        reader.MoveToElement();
    }
}
