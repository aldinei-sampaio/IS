using IS.Reading.Nodes;
using IS.Reading.Parsing.TextParsers;
using IS.Reading.Variables;
using System.Text.RegularExpressions;
using System.Xml;

namespace IS.Reading.Parsing.NodeParsers;

public class SetNodeParser : ISetNodeParser
{
    private readonly IElementParser elementParser;

    public IElementParserSettings Settings { get; }

    public SetNodeParser(
        IElementParser elementParser,
        IVarSetTextParser varSetTextParser
    )
    {
        this.elementParser = elementParser;
        Settings = ElementParserSettings.Normal(varSetTextParser);
    }

    public string Name => "set";

    public async Task ParseAsync(XmlReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext)
    {
        var myContext = new TextParentParsingContext();
        await elementParser.ParseAsync(reader, parsingContext, myContext, Settings);

        var parsedText = myContext.ParsedText;

        if (parsedText is null)
            return;

        var varSet = TryCreateIntegerNonNegativeSetVarSet(parsedText)
            ?? TryCreateIntegerNegativeSetVarSet(parsedText)
            ?? TryCreateIntegerIncrementVarSet(parsedText)
            ?? TryCreateStringVarSet(parsedText);

        if (varSet is null)
        {
            parsingContext.LogError(reader, "Expressão de atribuição de variável inválida.");
            return;
        }

        parentParsingContext.AddNode(new VarSetNode(varSet));
    }

    private static IVarSet? TryCreateIntegerNegativeSetVarSet(string parsedText)
    {
        var match = Regex.Match(parsedText, @"^(?<name>[A-Za-z_]+)\s?=\s?(?<value>-\d{1,9})$");
        if (!match.Success)
            return null;

        var name = match.Groups["name"].Value;
        var op = match.Groups["op"].Value;
        var value = int.Parse(match.Groups["value"].Value);

        return new VarSet(name, -value);
    }

    private static IVarSet? TryCreateIntegerNonNegativeSetVarSet(string parsedText)
    {
        var match = Regex.Match(parsedText, @"^(?<name>[A-Za-z_]+)\s?(?<op>=|\+=|-=)\s?(?<value>\d{1,9})$");
        if (!match.Success)
            return null;

        var name = match.Groups["name"].Value;
        var op = match.Groups["op"].Value;
        var value = int.Parse(match.Groups["value"].Value);

        if (op == "+=")
            return new IntegerIncrement(name, value);

        if (op == "-=")
            return new IntegerIncrement(name, -value);

        return new VarSet(name, value);
    }

    private static IVarSet? TryCreateIntegerIncrementVarSet(string parsedText)
    {
        var match = Regex.Match(parsedText, @"^(?<name>[A-Za-z_]+)\s?(?<op>\+\+|--)$");
        if (!match.Success)
            return null;

        var name = match.Groups["name"].Value;
        var op = match.Groups["op"].Value;

        return new IntegerIncrement(name, op == "++" ? 1 : -1);
    }

    private static IVarSet? TryCreateStringVarSet(string parsedText)
    {
        var match = Regex.Match(parsedText, @"^(?<name>[A-Za-z_]+)\s?=\s?'(?<value>[^']+)'$");
        if (!match.Success)
            return null;

        var name = match.Groups["name"].Value;
        var value = match.Groups["value"].Value;

        return new VarSet(name, value);
    }
}
