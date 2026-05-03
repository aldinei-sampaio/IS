using IS.App.Models;
using System.Net.Http.Json;

namespace IS.App.Services;

public interface IAssetManager
{
    Task<IReadOnlyList<CategoryModel>> GetCategoriesAsync();
    Task<IReadOnlyList<BookModel>> GetBooksAsync();
    string GetBookThumbnailUrl(string bookName);
}

public class AssetManager(HttpClient httpClient) : IAssetManager
{
    public async Task<IReadOnlyList<CategoryModel>> GetCategoriesAsync()
        => (await httpClient.GetFromJsonAsync<CategoryModel[]>("assets/books/categories.json"))!;

    public async Task<IReadOnlyList<BookModel>> GetBooksAsync()
        => (await httpClient.GetFromJsonAsync<BookModel[]>("assets/books/books.json"))!;

    public string GetBookThumbnailUrl(string bookName)
        => $"assets/books/{bookName}/thumbnail.png";
}
