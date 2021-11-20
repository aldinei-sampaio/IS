using System.Collections.Generic;

namespace IS.Reading.Parsing;

public class ParserDictionary<T> : Dictionary<string, T>
{
    public ParserDictionary() : base(StringComparer.OrdinalIgnoreCase)
    {
    }
}
