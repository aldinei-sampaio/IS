using System;
using System.Collections.Generic;

namespace IS.Reading
{
    public class StoryContext : IStoryContextUpdater, IStoryContextEvents
    {
        public Dictionary<string, string> Values = new Dictionary<string, string>();

        public event EventHandler<string>? OnMusicChange;
        public event EventHandler<string>? OnBackgroundChange;
        public event EventHandler? OnNarrationOpen;
        public event EventHandler<string>? OnNarrationChange;
        public event EventHandler? OnNarrationClose;
        public event EventHandler? OnTutorialOpen;
        public event EventHandler<string>? OnTutorialChange;
        public event EventHandler? OnTutorialClose;

        public event EventHandler<string>? OnProtagonistChange;
        public event EventHandler<string>? OnProtagonistArrive;
        public event EventHandler<string>? OnProtagonistFeelingChange;
        public event EventHandler? OnProtagonistThoughtOpen;
        public event EventHandler<string>? OnProtagonistThoughtChange;
        public event EventHandler? OnProtagonistThoughtClose;
        public event EventHandler? OnProtagonistSpeakOpen;
        public event EventHandler<string>? OnProtagonistSpeakChange;
        public event EventHandler? OnProtagonistSpeakClose;
        public event EventHandler? OnProtagonistLeave;

        public event EventHandler<Prompt>? OnPromptOpen;

        private string GetValue(string key)
        {
            if (Values.TryGetValue(key, out var value))
                return value ?? string.Empty;
            return string.Empty;
        }

        string IStoryContextUpdater.this[string key] {
            get => GetValue(key);
            set => Values[key] = value;
        }

        string IStoryContextUpdater.SetValue(string key, string newValue)
        {
            var oldValue = GetValue(key);
            Values[key] = newValue;
            return oldValue;
        }

        void IStoryContextUpdater.CallOnBackgroundChange(string name) => OnBackgroundChange?.Invoke(this, name);
        void IStoryContextUpdater.CallOnMusicChange(string name) => OnMusicChange?.Invoke(this, name);
        void IStoryContextUpdater.CallOnNarrationOpen() => OnNarrationOpen?.Invoke(this, EventArgs.Empty);
        void IStoryContextUpdater.CallOnNarrationChange(string name) => OnNarrationChange?.Invoke(this, name);
        void IStoryContextUpdater.CallOnNarrationClose() => OnNarrationClose?.Invoke(this, EventArgs.Empty);
        void IStoryContextUpdater.CallOnTutorialOpen() => OnTutorialOpen?.Invoke(this, EventArgs.Empty);
        void IStoryContextUpdater.CallOnTutorialChange(string name) => OnTutorialChange?.Invoke(this, name);
        void IStoryContextUpdater.CallOnTutorialClose() => OnTutorialClose?.Invoke(this, EventArgs.Empty);

        void IStoryContextUpdater.CallOnProtagonistChange(string name) => OnProtagonistChange?.Invoke(this, name);
        void IStoryContextUpdater.CallOnProtagonistArrive(string name) => OnProtagonistArrive?.Invoke(this, name);
        void IStoryContextUpdater.CallOnProtagonistFeelingChange(string name) => OnProtagonistFeelingChange?.Invoke(this, name);
        void IStoryContextUpdater.CallOnProtagonistSpeakOpen() => OnProtagonistSpeakOpen?.Invoke(this, EventArgs.Empty);
        void IStoryContextUpdater.CallOnProtagonistSpeakChange(string name) => OnProtagonistSpeakChange?.Invoke(this, name);
        void IStoryContextUpdater.CallOnProtagonistSpeakClose() => OnProtagonistSpeakClose?.Invoke(this, EventArgs.Empty);
        void IStoryContextUpdater.CallOnProtagonistThoughtOpen() => OnProtagonistThoughtOpen?.Invoke(this, EventArgs.Empty);
        void IStoryContextUpdater.CallOnProtagonistThoughtChange(string name) => OnProtagonistThoughtChange?.Invoke(this, name);
        void IStoryContextUpdater.CallOnProtagonistThoughtClose() => OnProtagonistThoughtClose?.Invoke(this, EventArgs.Empty);
        void IStoryContextUpdater.CallOnProtagonistLeave() => OnProtagonistLeave?.Invoke(this, EventArgs.Empty);

        void IStoryContextUpdater.CallOnPromptOpen(Prompt prompt) => OnPromptOpen?.Invoke(this, prompt);
    }
}
