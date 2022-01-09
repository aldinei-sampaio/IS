namespace IS.Reading.Variables;

public interface IVarSetParser
{
    IVarSet? Parse(string expression);
}
