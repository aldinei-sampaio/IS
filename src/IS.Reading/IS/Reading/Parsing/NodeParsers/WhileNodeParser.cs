using IS.Reading.Nodes;
using IS.Reading.Parsing.ConditionParsers;

namespace IS.Reading.Parsing.NodeParsers;

public class WhileNodeParser : IWhileNodeParser
{
    public WhileNodeParser(
         IElementParser elementParser,
         IConditionParser conditionParser,
         IElementParserSettingsFactory elementParserSettingsFactory
     )
    {
        ElementParser = elementParser;
        ConditionParser = conditionParser;
        ElementParserSettingsFactory = elementParserSettingsFactory;
    }

    public bool IsArgumentRequired => true;
    public string Name => "while";
    public IElementParser ElementParser { get; }
    public IConditionParser ConditionParser { get; }
    public IElementParserSettingsFactory ElementParserSettingsFactory { get; }

    public async Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var parsingResult = ConditionParser.Parse(reader.Argument);
        if (!parsingResult.IsOk)
        {
            parsingContext.LogError(reader, parsingResult.ErrorMessage);
            return;
        }

        var myContext = new ParentParsingContext();

        await ElementParser.ParseAsync(reader, parsingContext, myContext, ElementParserSettingsFactory.Block);

        if (!parsingContext.IsSuccess)
            return;

        var block = parsingContext.BlockFactory.Create(myContext.Nodes, parsingResult.Value);
        parentParsingContext.AddNode(new BlockNode(block));
    }
}