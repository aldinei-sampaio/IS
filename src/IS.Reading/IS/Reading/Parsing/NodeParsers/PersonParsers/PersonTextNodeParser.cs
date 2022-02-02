using IS.Reading.Parsing.ArgumentParsers;

namespace IS.Reading.Parsing.NodeParsers.PersonParsers;

public class PersonTextNodeParser : GenericTextNodeParserBase, IPersonTextNodeParser
{
    public PersonTextNodeParser(IElementParser elementParser, INameTextParser nameTextParser) 
        : base(elementParser, nameTextParser)
    {
    }

    public override string Name => "person";
}
