using IS.Reading.Navigation;
using System.IO;

namespace IS.Reading.Parsing
{
    public interface IStoryboardParser
    {
        Task<IStoryboard> ParseAsync(Stream stream);
    }
}