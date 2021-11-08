using IS.Blazor.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace IS.Blazor.Services
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

        public async Task<string> GetChapterDataAsync(string name, int chapter)
            => await (await httpClient.GetAsync($"/data/books/{name}/{chapter:000}/sc01.xml?{DateTime.Now:yyyyMMddHHmmss}")).Content.ReadAsStringAsync();

        public string GetBackgroundImageUrl(string imageName)
            => $"/data/assets/background/{imageName}.jpg";

        public async Task<Stream> GetStoryboardStreamAsync(string book, int chapter)
            => await (await httpClient.GetAsync($"/data/books/{book}/{chapter:000}/sc01.xml?{DateTime.Now:yyyyMMddHHmmss}")).Content.ReadAsStreamAsync();

        public async Task<Stream> GetThrophiesStreamAsync(string book, int chapter)
            => await (await httpClient.GetAsync($"/data/books/{book}/{chapter:000}/trophies.xml?{DateTime.Now:yyyyMMddHHmmss}")).Content.ReadAsStreamAsync();
    }
}
