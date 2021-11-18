using IS.Reading.Navigation;
using System.Xml.Linq;

namespace IS.Reading.Parsing;

public interface INodeParser
{
    INode Parse(XElement element);
}
