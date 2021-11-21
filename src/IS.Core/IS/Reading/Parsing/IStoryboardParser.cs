using IS.Reading.Navigation;

namespace IS.Reading.Parsing;

public interface IStoryboardParser
{
    Task<IStoryboard> ParseAsync(Stream stream);
}
