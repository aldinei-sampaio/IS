using System.Xml;

namespace IS.Reading.Parsing;

public interface IElementParser
{
    Task ParseAsync(
        XmlReader reader, 
        IParsingContext parsingContext, 
        IParentParsingContext parentParsingContext, 
        IElementParserSettings settings
    );
}
