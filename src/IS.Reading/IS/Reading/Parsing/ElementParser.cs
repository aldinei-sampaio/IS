using IS.Reading.Navigation;
using IS.Reading.Parsing.AttributeParsers;
using System.Xml;

namespace IS.Reading.Parsing;

public class ElementParser : IElementParser
{
    private class AggregateInfo
    {
        private readonly INodeParser nodeParser;
        public INodeAggregation Aggregation => nodeParser.NodeAggregation!;
        public ElementParsedData ParsedData { get; } = new();

        public AggregateInfo(INodeParser nodeParser)
            => this.nodeParser = nodeParser;

        public void Aggregate(ElementParsedData parsed)
        {
            if (ParsedData.Block is null)
                return;

            var node = nodeParser.Aggregate(ParsedData.Block);
            if (node is null)
                return;

            if (parsed.Block is null)
                parsed.Block = new Block();

            parsed.Block.ForwardQueue.Enqueue(node);
        }
    }

    public async Task<IElementParsedData> ParseAsync(XmlReader reader, IParsingContext parsingContext, IElementParserSettings settings)
    {
        var parsed = new ElementParsedData();

        if (reader.HasAttributes)
            ParseAttributes(reader, parsingContext, settings, parsed);

        var textFound = false;
        var elementFound = false;

        AggregateInfo? aggregateInfo = null;

        while (await reader.ReadAsync())
        {
            switch(reader.NodeType)
            {
                case XmlNodeType.Element:
                    elementFound = true;

                    if (aggregateInfo is not null)
                    {
                        if (aggregateInfo.Aggregation.ChildParsers.TryGet(reader.LocalName, out var aggParser))
                        {
                            await ParseElementAsync(reader, parsingContext, aggParser, aggregateInfo.ParsedData, textFound);
                            break;
                        }
                        aggregateInfo.Aggregate(parsed);
                        aggregateInfo = null;
                    }

                    if (!settings.ChildParsers.TryGet(reader.LocalName, out var parser))
                    {
                        parsingContext.LogError(reader, $"Elemento não reconhecido: {reader.LocalName}");
                        break;
                    }

                    if (parser.NodeAggregation is not null)
                    {
                        aggregateInfo = new(parser);
                        await ParseElementAsync(reader, parsingContext, parser, aggregateInfo.ParsedData, textFound);
                        break;
                    }

                    await ParseElementAsync(reader, parsingContext, parser, parsed, textFound);

                    break;

                case XmlNodeType.Text:
                    textFound = true;
                    ParseText(reader, parsingContext, settings, parsed, elementFound);
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

        aggregateInfo?.Aggregate(parsed);

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
        INodeParser parser,
        ElementParsedData parsed, 
        bool textFound
    )
    {
        if (textFound)
        {
            parsingContext.LogError(reader, "Não é permitido texto dentro de elemento que tenha elementos filhos.");
            return;
        }

        using var childReader = reader.ReadSubtree();
        await childReader.MoveToContentAsync();
        var child = await parser.ParseAsync(childReader, parsingContext);
        if (child is not null)
        {
            if (parsed.Block is null)
                parsed.Block = new Block();
            parsed.Block.ForwardQueue.Enqueue(child);

            var dismissNode = parser.DismissNode;
            if (dismissNode is not null)
            {
                if (!parsingContext.DismissNodes.Contains(dismissNode))
                    parsingContext.DismissNodes.Add(dismissNode);
            }
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
            if (!settings.AttributeParsers.TryGet(reader.LocalName, out var attributeParser))
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
