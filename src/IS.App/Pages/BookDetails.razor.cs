using IS.App.Models;
using IS.App.Services;
using Microsoft.AspNetCore.Components;

namespace IS.App.Pages;

public partial class BookDetails
{
    [Parameter]
    public string Name { get; set; } = string.Empty;

    [Inject]
    private IAssetManager AssetManager { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    private BookDetailsModel? details;
    private string coverUrl = string.Empty;
    private bool isMusicEnabled = true;

    protected override async Task OnParametersSetAsync()
    {
        details = null;
        coverUrl = AssetManager.GetBookCoverUrl(Name);
        details = await AssetManager.GetBookDetailsAsync(Name);
    }

    private void Close() => Navigation.NavigateTo("/");

    private void ToggleMusic() => isMusicEnabled = !isMusicEnabled;

    private void StartReading()
    {
        // Iniciar leitura (a implementar)
    }
}
