namespace IS.Reading.Variables;

public struct TextSourceParserResult : ITextSourceParserResult
{
    private readonly ITextSource? parsed = null;
    private readonly string? errorMessage = null;

    public bool IsError { get; } = false;

    public TextSourceParserResult(ITextSource? parsed)
        => this.parsed = parsed;

    public TextSourceParserResult(string? errorMessage)
    {
        this.errorMessage = errorMessage;
        IsError = true;
    }

    public ITextSource TextSource => parsed ?? throw new InvalidOperationException();

    public string ErrorMessage => errorMessage ?? throw new InvalidOperationException();
}
