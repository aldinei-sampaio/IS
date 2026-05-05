using IS.App.Services;
using IS.Reading.Navigation;
using IS.Reading.Parsing;
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
    private IStoryboardParser StoryboardParser { get; set; } = default!;

    [Inject]
    private HttpClient HttpClient { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    private string coverUrl = string.Empty;
    private string statusMessage = string.Empty;
    private IStoryboard? storyboard;

    protected override async Task OnParametersSetAsync()
    {
        storyboard = null;
        coverUrl = AssetManager.GetBookCoverUrl(BookId);
        statusMessage = $"Carregando capítulo {Chapter}...";
        StateHasChanged();

        try
        {
            var url = AssetManager.GetChapterUrl(BookId, Chapter);
            var content = await HttpClient.GetStringAsync(url);
            using var lineReader = new DocumentLineReader(new StringReader(content));
            var docReader = new DocumentReader(lineReader);
            storyboard = await StoryboardParser.ParseAsync(docReader);
            statusMessage = $"Capítulo {Chapter} carregado com sucesso. Toque para sair.";
        }
        catch
        {
            statusMessage = $"Erro ao carregar o capítulo {Chapter}. Toque para sair.";
        }
    }

    private void Exit() => Navigation.NavigateTo($"/book/{BookId}");
}
