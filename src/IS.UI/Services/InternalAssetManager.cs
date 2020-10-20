using IS.UI.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace IS.UI.Services
{
    public class InternalAssetManager : IAssetManager
    {
        private readonly HttpClient httpClient;

        public InternalAssetManager(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IReadOnlyList<BookModel>> GetBooksAsync()
            => await httpClient.GetFromJsonAsync<BookModel[]>("data/books/books.json");

        public string GetBookCoverUrl(string bookName)
            => $"data/books/{bookName}/cover.jpg";
    }
}
