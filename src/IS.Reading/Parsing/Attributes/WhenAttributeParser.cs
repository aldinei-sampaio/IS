using IS.Reading.Conditions;
using System.Xml;

namespace IS.Reading.Parsing.Attributes
{
    public class WhenAttributeParser : IAttributeParser
    {
        public static WhenAttributeParser Instance { get; } = new();
        
        public string ElementName => "when";

        public IAttribute? Parse(XmlReader reader, IParsingContext parsingContext)
        {
            if (ConditionParser.TryParse(reader.Value, out var condition))
                return new WhenAttribute(condition);

            parsingContext.LogError(reader, "Condição 'when' inválida.");
            return null;
        }
    }
}
