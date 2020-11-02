using System.Collections.Generic;
using System.Text;

namespace IS.Reading.StoryboardNavigatorTests
{
    public class EventListener
    {
        private StringBuilder builder = new StringBuilder();

        private void Add(string text) => builder.AppendLine(text);

        public string Forward(Storyboard sb)
        {
            builder = new StringBuilder();
            while (sb.MoveNext()) { }
            return builder.ToString();
        }

        public string Backward(Storyboard sb)
        {
            builder = new StringBuilder();
            while (sb.MovePrevious()) { }
            return builder.ToString();
        }

        public EventListener(IStoryContextEvents contextEvents)
        {
            contextEvents.Navigation.OnMoveNext += (s, e) => Add("-- next --");
            contextEvents.Navigation.OnMovePrevious += (s, e) => Add("-- previous --");

            contextEvents.Background.OnChange += (s, e) => Add($"OnBackgroundChange({e})");
            contextEvents.Music.OnChange += (s, e) => Add($"OnMusicChange({e})");

            contextEvents.Display.OnOpen += (s, e) => Add($"OnDisplayOpen({e})");

            contextEvents.Tutorial.OnOpen += (s, e) => Add($"OnTutorialOpen()");
            contextEvents.Tutorial.OnChange += (s, e) => Add($"OnTutorialChange({e})");
            contextEvents.Tutorial.OnClose += (s, e) => Add($"OnTutorialClose()");

            contextEvents.Narration.OnOpen += (s, e) => Add($"OnNarrationOpen()");
            contextEvents.Narration.OnChange += (s, e) => Add($"OnNarrationChange({e})");
            contextEvents.Narration.OnClose += (s, e) => Add($"OnNarrationClose()");

            contextEvents.Protagonist.OnChange += (s, e) => Add($"OnProtagonistChange({e})");
            contextEvents.Protagonist.OnArrive += (s, e) => Add($"OnProtagonistArrive({e})");
            contextEvents.Protagonist.OnBump += (s, e) => Add($"OnProtagonistBump()");
            contextEvents.Protagonist.OnLeave += (s, e) => Add($"OnProtagonistLeave()");
            contextEvents.Protagonist.Mood.OnChange += (s, e) => Add($"OnProtagonistMoodChange({e})");
            contextEvents.Protagonist.Reward.OnOpen += (s, e) => Add($"OnProtagonistRewardOpen({e})");
            contextEvents.Protagonist.Speech.OnOpen += (s, e) => Add($"OnProtagonistSpeechOpen()");
            contextEvents.Protagonist.Speech.OnChange += (s, e) => Add($"OnProtagonistSpeechChange({e})");
            contextEvents.Protagonist.Speech.OnClose += (s, e) => Add($"OnProtagonistSpeechClose()");
            contextEvents.Protagonist.Thought.OnOpen += (s, e) => Add($"OnProtagonistThoughtOpen()");
            contextEvents.Protagonist.Thought.OnChange += (s, e) => Add($"OnProtagonistThoughtChange({e})");
            contextEvents.Protagonist.Thought.OnClose += (s, e) => Add($"OnProtagonistThoughtClose()");

            contextEvents.Interlocutor.OnArrive += (s, e) => Add($"OnInterlocutorArrive({e})");
            contextEvents.Interlocutor.OnBump += (s, e) => Add($"OnInterlocutorBump()");
            contextEvents.Interlocutor.OnLeave += (s, e) => Add($"OnInterlocutorLeave()");
            contextEvents.Interlocutor.Mood.OnChange += (s, e) => Add($"OnInterlocutorMoodChange({e})");
            contextEvents.Interlocutor.Reward.OnOpen += (s, e) => Add($"OnInterlocutorRewardOpen({e})");
            contextEvents.Interlocutor.Speech.OnOpen += (s, e) => Add($"OnInterlocutorSpeechOpen()");
            contextEvents.Interlocutor.Speech.OnChange += (s, e) => Add($"OnInterlocutorSpeechChange({e})");
            contextEvents.Interlocutor.Speech.OnClose += (s, e) => Add($"OnInterlocutorSpeechClose()");
            contextEvents.Interlocutor.Thought.OnOpen += (s, e) => Add($"OnInterlocutorThoughtOpen()");
            contextEvents.Interlocutor.Thought.OnChange += (s, e) => Add($"OnInterlocutorThoughtChange({e})");
            contextEvents.Interlocutor.Thought.OnClose += (s, e) => Add($"OnInterlocutorThoughtClose()");

            contextEvents.Prompt.OnOpen += (s, e) => Add($"OnPromptOpen()");
        }
    }
}
