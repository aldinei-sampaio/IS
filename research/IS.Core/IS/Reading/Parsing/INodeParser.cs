namespace IS.Reading.Parsing;

public interface INodeParser : IParser
{
    bool IsArgumentRequired { get; }
    Task ParseAsync(IDocumentReader reader, IParsingContext parsingContext, IParentParsingContext parentParsingContext);
}
