using IS.Blazor.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace IS.Blazor.Services
{
    public interface IAssetManager
    {
        Task<BookModel> GetBookAsync(string name);
        string GetBookCoverUrl(string bookName);
        Task<IReadOnlyList<BookModel>> GetBooksAsync();
        Task<string> GetChapterDataAsync(string name, int chapter);

        string GetBackgroundImageUrl(string imageName);
        Task<Stream> GetStoryboardStreamAsync(string book, int chapter);
        Task<Stream> GetThrophiesStreamAsync(string book, int chapter);
    }
}
