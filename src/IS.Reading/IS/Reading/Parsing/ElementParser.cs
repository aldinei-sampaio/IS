using IS.Reading.Navigation;
using IS.Reading.Parsing.Attributes;
using System.Xml;

namespace IS.Reading.Parsing;

public class ElementParser : IElementParser
{
    public ITextParser? TextParser { get; set; }
    public ParserDictionary<IAttributeParser> AttributeParsers { get; } = new();
    public ParserDictionary<INodeParser> ChildParsers { get; } = new();

    public async Task<IElementParsedData?> ParseAsync(XmlReader reader, IParsingContext parsingContext)
    {
        var parsed = new ElementParsedData();

        if (reader.HasAttributes)
        {
            while (reader.MoveToNextAttribute())
            {
                if (!AttributeParsers.TryGetValue(reader.LocalName, out var attributeParser))
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

        var textFound = false;
        var elementFound = false;

        while (await reader.ReadAsync())
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                if (textFound)
                {
                    parsingContext.LogError(reader, "Não é permitido texto dentro de elemento que tenha elementos filhos.");
                    continue;
                }

                elementFound = true;

                if (!ChildParsers.TryGetValue(reader.LocalName, out var parser))
                {
                    parsingContext.LogError(reader, $"Elemento não reconhecido: {reader.LocalName}");
                    continue;
                }

                using var childReader = reader.ReadSubtree();
                var child = await parser.ParseAsync(childReader, parsingContext);
                if (child is not null)
                {
                    if (parsed.Block is null)
                        parsed.Block = new Block();
                    parsed.Block.ForwardQueue.Enqueue(child);
                }
            }
            else if (reader.NodeType == XmlNodeType.Text)
            {
                if (elementFound)
                {
                    parsingContext.LogError(reader, "Não é permitido texto dentro de elemento que tenha elementos filhos.");
                    continue;
                }

                textFound = true;

                if (TextParser is null)
                {
                    parsingContext.LogError(reader, "Este elemento não permite texto.");
                    continue;
                }

                var parsedText = TextParser.Parse(reader, parsingContext, reader.Value);
                if (parsedText is not null)
                    parsed.Text = parsedText;
            }
            else if (reader.NodeType != XmlNodeType.EndElement)
            {
                parsingContext.LogError(reader, $"Conteúdo inválido detectado: {reader.NodeType}");
            }
        }

        return parsed;
    }
}
