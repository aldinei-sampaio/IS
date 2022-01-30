using IS.Reading.Navigation;
using System.Xml;

namespace IS.Reading.Parsing;

public interface INodeParser : IParser
{
    Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext);
}
