using System.Text;

namespace IS.Reading.StoryboardNavigatorTests;

public class EventListener
{
    private StringBuilder builder = new StringBuilder();

    private Task AddAsync(string text)
    { 
        builder.AppendLine(text);
        return Task.CompletedTask;
    }

    public async Task<string> ForwardAsync(Storyboard sb)
    {
        builder = new StringBuilder();
        while (await sb.MoveNextAsync()) { }
        return builder.ToString();
    }

    public async Task<string> BackwardAsync(Storyboard sb)
    {
        builder = new StringBuilder();
        while (await sb.MovePreviousAsync()) { }
        return builder.ToString();
    }

    public EventListener(IStoryContextEvents contextEvents)
    {
        contextEvents.Navigation.OnMoveNextAsync += (s, e) => AddAsync("-- next --");
        contextEvents.Navigation.OnMovePreviousAsync += (s, e) => AddAsync("-- previous --");

        contextEvents.Background.OnChangeAsync += (s, e) => AddAsync($"OnBackgroundChange({e})");
        contextEvents.Music.OnChangeAsync += (s, e) => AddAsync($"OnMusicChange({e})");

        contextEvents.Display.OnOpenAsync += (s, e) => AddAsync($"OnDisplayOpen({e})");

        contextEvents.Tutorial.OnOpenAsync += (s, e) => AddAsync($"OnTutorialOpen()");
        contextEvents.Tutorial.OnChangeAsync += (s, e) => AddAsync($"OnTutorialChange({e})");
        contextEvents.Tutorial.OnCloseAsync += (s, e) => AddAsync($"OnTutorialClose()");

        contextEvents.Narration.OnOpenAsync += (s, e) => AddAsync($"OnNarrationOpen()");
        contextEvents.Narration.OnChangeAsync += (s, e) => AddAsync($"OnNarrationChange({e})");
        contextEvents.Narration.OnCloseAsync += (s, e) => AddAsync($"OnNarrationClose()");

        contextEvents.Protagonist.OnChangeAsync += (s, e) => AddAsync($"OnProtagonistChange({e})");
        contextEvents.Protagonist.OnArriveAsync += (s, e) => AddAsync($"OnProtagonistArrive({e})");
        contextEvents.Protagonist.OnBumpAsync += (s, e) => AddAsync($"OnProtagonistBump()");
        contextEvents.Protagonist.OnLeaveAsync += (s, e) => AddAsync($"OnProtagonistLeave()");
        contextEvents.Protagonist.Mood.OnChangeAsync += (s, e) => AddAsync($"OnProtagonistMoodChange({e})");
        contextEvents.Protagonist.Reward.OnOpenAsync += (s, e) => AddAsync($"OnProtagonistRewardOpen({e})");
        contextEvents.Protagonist.Speech.OnOpenAsync += (s, e) => AddAsync($"OnProtagonistSpeechOpen()");
        contextEvents.Protagonist.Speech.OnChangeAsync += (s, e) => AddAsync($"OnProtagonistSpeechChange({e})");
        contextEvents.Protagonist.Speech.OnCloseAsync += (s, e) => AddAsync($"OnProtagonistSpeechClose()");
        contextEvents.Protagonist.Thought.OnOpenAsync += (s, e) => AddAsync($"OnProtagonistThoughtOpen()");
        contextEvents.Protagonist.Thought.OnChangeAsync += (s, e) => AddAsync($"OnProtagonistThoughtChange({e})");
        contextEvents.Protagonist.Thought.OnCloseAsync += (s, e) => AddAsync($"OnProtagonistThoughtClose()");

        contextEvents.Interlocutor.OnArriveAsync += (s, e) => AddAsync($"OnInterlocutorArrive({e})");
        contextEvents.Interlocutor.OnBumpAsync += (s, e) => AddAsync($"OnInterlocutorBump()");
        contextEvents.Interlocutor.OnLeaveAsync += (s, e) => AddAsync($"OnInterlocutorLeave()");
        contextEvents.Interlocutor.Mood.OnChangeAsync += (s, e) => AddAsync($"OnInterlocutorMoodChange({e})");
        contextEvents.Interlocutor.Reward.OnOpenAsync += (s, e) => AddAsync($"OnInterlocutorRewardOpen({e})");
        contextEvents.Interlocutor.Speech.OnOpenAsync += (s, e) => AddAsync($"OnInterlocutorSpeechOpen()");
        contextEvents.Interlocutor.Speech.OnChangeAsync += (s, e) => AddAsync($"OnInterlocutorSpeechChange({e})");
        contextEvents.Interlocutor.Speech.OnCloseAsync += (s, e) => AddAsync($"OnInterlocutorSpeechClose()");
        contextEvents.Interlocutor.Thought.OnOpenAsync += (s, e) => AddAsync($"OnInterlocutorThoughtOpen()");
        contextEvents.Interlocutor.Thought.OnChangeAsync += (s, e) => AddAsync($"OnInterlocutorThoughtChange({e})");
        contextEvents.Interlocutor.Thought.OnCloseAsync += (s, e) => AddAsync($"OnInterlocutorThoughtClose()");

        contextEvents.Prompt.OnOpenAsync += (s, e) => AddAsync($"OnPromptOpen()");
    }
}
