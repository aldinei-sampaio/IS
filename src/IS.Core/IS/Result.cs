namespace IS;

public static class Result
{
    public static Result<T> Ok<T>(T value) => new(value);
    public static Result<T> Fail<T>(string message) => new(message);
}

public struct Result<T>
{
    public T Value { get; } = default!;
    public bool IsOk { get; }
    public string ErrorMessage { get; } = string.Empty;

    public Result(T value)
    {
        Value = value;
        IsOk = true;
    }

    public Result(string errorMessage)
    {
        ErrorMessage = errorMessage;
        IsOk = false;
    }
}
