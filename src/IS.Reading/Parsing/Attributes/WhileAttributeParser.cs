using IS.Reading.Conditions;
using System.Xml;

namespace IS.Reading.Parsing.Attributes
{
    public class WhileAttributeParser : IAttributeParser
    {
        public static WhileAttributeParser Instance { get; } = new();

        public string ElementName => "when";

        public IAttribute? Parse(XmlReader reader, IParsingContext parsingContext)
        {
            if (ConditionParser.TryParse(reader.Value, out var condition))
                return new WhileAttribute(condition);

            parsingContext.LogError(reader, "Condição 'while' inválida.");
            return null;
        }
    }
}
