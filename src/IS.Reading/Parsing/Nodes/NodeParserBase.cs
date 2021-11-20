using IS.Reading.Navigation;
using IS.Reading.Parsing.Attributes;
using IS.Reading.Parsing.Text;
using System.Xml;

namespace IS.Reading.Parsing.Nodes;

public abstract class NodeParserBase : INodeParser
{
    public abstract string ElementName { get; }

    protected ITextParser? TextParser { get; set; }
    protected ParserDictionary<IAttributeParser> AttributeParsers { get; } = new();
    protected ParserDictionary<INodeParser> ChildParsers { get; } = new();

    protected abstract INode? CreateNode(XmlReader reader, IParsingContext parsingContext, ParsedData parsed);

    public async Task<INode?> ParseAsync(XmlReader reader, IParsingContext parsingContext)
    {
        var parsed = new ParsedData();

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
        IBlock? block = null;

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

                var child = await parser.ParseAsync(reader, parsingContext);
                if (child is not null)
                {
                    if (block is null)
                        block = new Block();
                    block.Add(child);
                }
            }
            else if (reader.NodeType == XmlNodeType.Text)
            {
                if (elementFound)
                {
                    parsingContext.LogError(reader, "Não é permitido texto dentro de elemento que tenha elementos filhos.");
                    continue;
                }

                if (textFound)
                {
                    parsingContext.LogError(reader, "Mais de um bloco de texto encontrado.");
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

        return CreateNode(reader, parsingContext, parsed);
    }
}
