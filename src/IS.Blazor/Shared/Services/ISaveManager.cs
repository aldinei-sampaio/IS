using IS.Blazor.Dto;
using System.Threading.Tasks;

namespace IS.Blazor.Services
{
    public interface ISaveManager
    {
        Task<BookProgressDto?> GetBookProgressAsync(string bookName);
    }
}
