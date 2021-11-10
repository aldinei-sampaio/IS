using IS.Blazor.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IEnumerable<CategoryDto>> GetBooksByCategoryAsync()
        {
            var books = await httpClient.GetFromJsonAsync<IEnumerable<BookDto>>($"assets/books/books.json?{DateTime.Now:yyyyMMddHHmmss}");
            if (books == null)
                return Enumerable.Empty<CategoryDto>();

            var categories = await httpClient.GetFromJsonAsync<List<CategoryDto>>($"assets/books/categories.json?{DateTime.Now:yyyyMMddHHmmss}");
            if (categories == null)
                return Enumerable.Empty<CategoryDto>();

            foreach (var category in categories)
            {
                var categoryBooks = new List<BookDto>();
                foreach(var book in books)
                {
                    if (book.Categories.Contains(category.Name, StringComparer.OrdinalIgnoreCase))
                        categoryBooks.Add(book);
                }
                category.Books = categoryBooks;
            }

            return categories;
        }

        public string GetBookThumbnailUrl(string bookName)
            => (new Uri(httpClient.BaseAddress!, $"assets/books/{bookName}/thumbnail.png")).ToString();

        public string GetCommonIconUrl(string imageName)
            => (new Uri(httpClient.BaseAddress!, $"assets/common/{imageName}.png")).ToString();

    }
}
