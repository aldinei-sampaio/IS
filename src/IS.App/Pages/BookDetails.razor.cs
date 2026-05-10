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
    private IAudioService AudioService { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    private BookDetailsModel? details;
    private string coverUrl = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await AudioService.InitializeAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        details = null;
        coverUrl = AssetManager.GetBookCoverUrl(Name);
        details = await AssetManager.GetBookDetailsAsync(Name);
    }

    private void Close() => Navigation.NavigateTo("/");

    private async Task ToggleMusic() => await AudioService.SetEnabledAsync(!AudioService.IsEnabled, null);

    private void StartReading() => Navigation.NavigateTo($"/book/{Name}/read/1");
}
