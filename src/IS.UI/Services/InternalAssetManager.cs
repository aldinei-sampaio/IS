using IS.UI.Models;
using System;
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
            => await httpClient.GetFromJsonAsync<BookModel[]>($"data/books/books.json?{DateTime.Now:yyyyMMddHHmmss}");

        public string GetBookCoverUrl(string bookName)
            => $"data/books/{bookName}/cover.jpg";

        public async Task<BookModel> GetBookAsync(string name)
            => await httpClient.GetFromJsonAsync<BookModel>($"data/books/{name}/book.json?{DateTime.Now:yyyyMMddHHmmss}");

        public async Task<string> GetChapterData(string name, int chapter)
            => await (await httpClient.GetAsync($"/data/books/{name}/{chapter:000}/sc01.xml")).Content.ReadAsStringAsync();
    }
}
