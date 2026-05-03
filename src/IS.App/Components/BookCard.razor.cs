using IS.App.Models;
using Microsoft.AspNetCore.Components;

namespace IS.App.Components;

public partial class BookCard
{
    [Parameter, EditorRequired]
    public BookModel Book { get; set; } = default!;

    [Parameter, EditorRequired]
    public string ThumbnailUrl { get; set; } = string.Empty;

    [Parameter]
    public EventCallback<BookModel> OnSelected { get; set; }

    private Task OnClick() => OnSelected.InvokeAsync(Book);
}
