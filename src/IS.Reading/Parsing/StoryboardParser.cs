using IS.Reading.Navigation;
using IS.Reading.Parsing.Nodes;
using System.IO;
using System.Xml;

namespace IS.Reading.Parsing;

public class StoryboardParser
{
    private readonly INodeParser nodeParser;

    public StoryboardParser(INodeParser nodeParser)
    {
        this.nodeParser = nodeParser;
    }

    public async Task<IStoryboard> ParseAsync(Stream stream)
    {
        var context = new ParsingContext();
        var xmlReader = XmlReader.Create(stream);
        await xmlReader.MoveToContentAsync();

        var node = await nodeParser.ParseAsync(xmlReader, context);

        if (!context.IsSuccess)
            throw new ParsingException(context.ToString());

        if (node is null || node.ChildBlock is null)
            throw new InvalidOperationException();

        return new Storyboard(node.ChildBlock);
    }
}
