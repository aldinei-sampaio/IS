using IS.Reading.Navigation;
using IS.Reading.Parsing.NodeParsers;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Linq;

namespace IS.Reading.Parsing;

public class StoryboardParser
{
    private readonly Dictionary<string, INodeParser> parsers = CreateParsers();

    private static Dictionary<string, INodeParser> CreateParsers()
    {
        return new(StringComparer.OrdinalIgnoreCase)
        {
            { "background", new BackgroundParser() }
        };
    }

    public async Task<IStoryboard> ParseAsync(Stream stream)
    {
        var xmlReader = XmlReader.Create(stream);
        await xmlReader.MoveToContentAsync();

        var block = new Block();

        while (await xmlReader.ReadAsync())
        {
            if (xmlReader.NodeType == XmlNodeType.Element)
            {
                if (!parsers.TryGetValue(xmlReader.LocalName, out var parser))
                    throw new ElementNotRecognizedException(xmlReader.LocalName);

                using var elementReader = xmlReader.ReadSubtree();
                var element = await XElement.LoadAsync(elementReader, LoadOptions.None, CancellationToken.None);

                block.ForwardQueue.Enqueue(parser.Parse(element));
            }
        }

        return new Storyboard(block);
    }
}
