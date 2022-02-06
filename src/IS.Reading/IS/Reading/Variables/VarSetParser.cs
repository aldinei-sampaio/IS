using System.Text.RegularExpressions;

namespace IS.Reading.Variables;

public class VarSetParser : IVarSetParser
{
    public Result<IVarSet> Parse(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
            return Result.Fail<IVarSet>("Expressão não informada.");

        var result = TryCreateIntegerSetVarSet(expression)
            ?? TryCreateIntegerIncrementVarSet(expression)
            ?? TryCreateStringVarSet(expression);

        if (result is null)
            return Result.Fail<IVarSet>("Expressão de atribuição inválida.");

        return Result.Ok(result);
    }

    private static IVarSet? TryCreateIntegerSetVarSet(string parsedText)
    {
        var match = Regex.Match(parsedText, @"^\s*(?<name>[A-Za-z_]+)\s*(?<op>=|\+=|-=)\s*(?<value>-?\d{1,10})\s*$");
        if (!match.Success)
            return null;

        var name = match.Groups["name"].Value.ToLower();
        var op = match.Groups["op"].Value;
        if (!int.TryParse(match.Groups["value"].Value, out var value))
            return null;

        if (op == "+=")
            return new IntegerIncrement(name, value);

        if (op == "-=")
            return new IntegerIncrement(name, -value);

        return new VarSet(name, value);
    }

    private static IVarSet? TryCreateIntegerIncrementVarSet(string parsedText)
    {
        var match = Regex.Match(parsedText, @"^\s*(?<name>[A-Za-z_]+)\s*(?<op>\+\+|--)\s*$");
        if (!match.Success)
            return null;

        var name = match.Groups["name"].Value.ToLower();
        var op = match.Groups["op"].Value;

        return new IntegerIncrement(name, op == "++" ? 1 : -1);
    }

    private static IVarSet? TryCreateStringVarSet(string parsedText)
    {
        var match = Regex.Match(parsedText, @"^\s*(?<name>[A-Za-z_]+)\s*=\s*'(?<value>[^']*)'\s*$");
        if (!match.Success)
            return null;

        var name = match.Groups["name"].Value.ToLower();
        var value = match.Groups["value"].Value;

        return new VarSet(name, value);
    }
}
