using IS.Reading.Navigation;
using System.Xml;

namespace IS.Reading.Parsing;

public interface IParsingContext
{
    void LogError(XmlReader xmlReader, string message);
    bool IsSuccess { get; }
    List<INode> DismissNodes { get; }
}
