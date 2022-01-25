namespace IS.Reading.Variables;

public interface ITextSourceParserResult
{
    bool IsError { get; }
    ITextSource TextSource { get; }
    string ErrorMessage { get; }
}
