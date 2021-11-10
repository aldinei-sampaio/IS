using IS.Blazor.Dto;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace IS.Blazor.Services
{
    public interface IAssetManager
    {
        Task<IEnumerable<CategoryDto>> GetBooksByCategoryAsync();
        string GetBookThumbnailUrl(string bookName);
        string GetCommonIconUrl(string imageName);
    }
}
