using IS.Reading.Navigation;
using System.Xml;

namespace IS.Reading.Parsing;

public class FakeNodeParser : INodeParser
{
    private readonly INode ret;

    public FakeNodeParser(string name, INode ret)
    {
        this.ret = ret;
        Name = name;
    }

    public string Name { get; }

    public async Task ParseAsync(XmlReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        while (await reader.ReadAsync())
        {
        }
        if (ret is not null)
            parentParsingContext.AddNode(ret);
    }
}
