namespace IS.Reading.Variables;

public interface IVarSetParser
{
    Result<IVarSet> Parse(string value);
}
