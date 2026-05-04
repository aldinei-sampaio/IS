using IS.App.Services;
using IS.Reading;
using Microsoft.AspNetCore.Components;

namespace IS.App.Pages;

public partial class BookReader
{
    [Parameter]
    public string BookId { get; set; } = string.Empty;

    [Parameter]
    public int Chapter { get; set; }

    [Inject]
    private IAssetManager AssetManager { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    private string coverUrl = string.Empty;
    private string statusMessage = string.Empty;
    private StoryBoard? storyBoard;

    protected override async Task OnParametersSetAsync()
    {
        storyBoard = null;
        coverUrl = AssetManager.GetBookCoverUrl(BookId);
        statusMessage = $"Carregando capítulo {Chapter}...";
        StateHasChanged();

        try
        {
            storyBoard = await AssetManager.GetChapterAsync(BookId, Chapter);
            statusMessage = $"Capítulo {Chapter} carregado com sucesso. Toque para sair.";
        }
        catch
        {
            statusMessage = $"Erro ao carregar o capítulo {Chapter}. Toque para sair.";
        }
    }

    private void Exit() => Navigation.NavigateTo($"/book/{BookId}");
}
