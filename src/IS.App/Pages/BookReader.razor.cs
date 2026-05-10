using IS.App.Components;
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

    private string statusMessage = string.Empty;
    private IStoryboard? storyboard;
    private int slideCount;
    private bool isAtEnd;
    private ReaderControls? readerControls;

    protected override async Task OnParametersSetAsync()
    {
        storyboard?.Dispose();
        storyboard = null;
        slideCount = 0;
        isAtEnd = false;
        statusMessage = $"Carregando capítulo {Chapter}...";
        StateHasChanged();

        try
        {
            var url = AssetManager.GetChapterUrl(BookId, Chapter);
            var content = await HttpClient.GetStringAsync(url);
            using var lineReader = new DocumentLineReader(new StringReader(content));
            var docReader = new DocumentReader(lineReader);
            storyboard = await StoryboardParser.ParseAsync(docReader);
            StateHasChanged();   // renders Background so it subscribes before Move is called
            await Task.Yield();  // yields to the event loop, allowing Blazor to render
            await AdvanceAsync();
        }
        catch
        {
            statusMessage = $"Erro ao carregar o capítulo {Chapter}. Toque para sair.";
        }
    }

    private async Task AdvanceAsync()
    {
        var hasMore = await storyboard!.MoveAsync(true);
        slideCount++;
        if (!hasMore)
            isAtEnd = true;
        readerControls?.OnSlideChanged();
    }

    private async Task OnTap()
    {
        if (storyboard == null || isAtEnd)
        {
            storyboard?.Dispose();
            Navigation.NavigateTo($"/book/{BookId}");
            return;
        }
        await AdvanceAsync();
    }
}
