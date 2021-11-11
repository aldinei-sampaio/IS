using IS.Blazor.Dto;
using System.Threading.Tasks;

namespace IS.Blazor.Services
{
    public class InternalSaveManager : ISaveManager
    {
        public Task<BookProgressDto?> GetBookProgressAsync(string bookName)
        {
            BookProgressDto? result = null;
            return Task.FromResult(result);
        }
    }
}
