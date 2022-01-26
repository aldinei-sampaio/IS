namespace IS.Reading.Variables;

public interface IInterpolator
{
    IEnumerable<IInterpolatedValue> Values { get; }
    string ToString(IVariableDictionary variables);
}
