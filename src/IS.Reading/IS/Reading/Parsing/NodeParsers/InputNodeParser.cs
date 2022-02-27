using IS.Reading.Nodes;
using IS.Reading.Parsing.ArgumentParsers;
using IS.Reading.Parsing.NodeParsers.InputParsers;

namespace IS.Reading.Parsing.NodeParsers;

public class InputNodeParser : IInputNodeParser
{
    public IElementParser ElementParser { get; }
    public INameArgumentParser NameArgumentParser { get; }
    public IElementParserSettings Settings { get; }

    public InputNodeParser(
        IElementParser elementParser,
        INameArgumentParser nameArgumentParser,
        IInputTitleNodeParser inputTitleNodeParser,
        IInputTextNodeParser inputTextNodeParser,
        IInputLenNodeParser inputLenNodeParser,
        IInputConfNodeParser inputConfNodeParser
    )
    {
        ElementParser = elementParser;
        NameArgumentParser = nameArgumentParser;
        Settings = new ElementParserSettings.AggregatedNonRepeat(
            inputTitleNodeParser,
            inputTextNodeParser, 
            inputLenNodeParser, 
            inputConfNodeParser
        );
    }

    public bool IsArgumentRequired => true;

    public string Name => "input";

    public async Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var result = NameArgumentParser.Parse(reader.Argument);
        if (!result.IsOk)
        {
            parsingContext.LogError(reader, result.ErrorMessage);
            return;
        }

        var myContext = new InputParentParsingContext(result.Value);
        await ElementParser.ParseAsync(reader, parsingContext, myContext, Settings);

        if (!parsingContext.IsSuccess)
            return;

        parentParsingContext.AddNode(new InputNode(myContext.InputBuilder));
    }
}