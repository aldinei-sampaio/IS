using IS.App.Models;
using IS.App.Services;
using Microsoft.AspNetCore.Components;

namespace IS.App.Pages;

public partial class Library
{
    [Inject]
    private IAssetManager AssetManager { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    private IReadOnlyList<CategoryModel>? categories;
    private IReadOnlyList<BookModel>? books;

    protected override async Task OnInitializedAsync()
    {
        var categoriesTask = AssetManager.GetCategoriesAsync();
        var booksTask = AssetManager.GetBooksAsync();
        await Task.WhenAll(categoriesTask, booksTask);
        categories = categoriesTask.Result;
        books = booksTask.Result;
    }

    private IEnumerable<BookModel> GetBooksByCategory(string categoryId)
        => books?.Where(b => b.Categories.Contains(categoryId)) ?? [];

    private void OnBookSelected(BookModel book)
        => Navigation.NavigateTo($"/book/{book.Id}");
}
