namespace IS.Reading.Variables;

public interface IInterpolatedValue
{
    string ToString(IVariableDictionary variables);
}
