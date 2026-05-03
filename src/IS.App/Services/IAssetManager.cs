using IS.App.Models;
using System.Net.Http.Json;

namespace IS.App.Services;

public interface IAssetManager
{
    Task<IReadOnlyList<CategoryModel>> GetCategoriesAsync();
    Task<IReadOnlyList<BookModel>> GetBooksAsync();
    Task<BookDetailsModel> GetBookDetailsAsync(string bookName);
    string GetBookThumbnailUrl(string bookName);
    string GetBookCoverUrl(string bookName);
}

public class AssetManager(HttpClient httpClient) : IAssetManager
{
    public async Task<IReadOnlyList<CategoryModel>> GetCategoriesAsync()
        => (await httpClient.GetFromJsonAsync<CategoryModel[]>("assets/books/categories.json"))!;

    public async Task<IReadOnlyList<BookModel>> GetBooksAsync()
        => (await httpClient.GetFromJsonAsync<BookModel[]>("assets/books/books.json"))!;

    public async Task<BookDetailsModel> GetBookDetailsAsync(string bookName)
        => (await httpClient.GetFromJsonAsync<BookDetailsModel>($"assets/books/{bookName}/details.json"))!;

    public string GetBookThumbnailUrl(string bookName)
        => $"assets/books/{bookName}/thumbnail.png";

    public string GetBookCoverUrl(string bookName)
        => $"assets/books/{bookName}/cover.png";
}
