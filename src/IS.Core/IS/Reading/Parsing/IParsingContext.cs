using System.Xml;

namespace IS.Reading.Parsing;

public interface IParsingContext
{
    void LogError(XmlReader xmlReader, string message);
    bool IsSuccess { get; }
}
