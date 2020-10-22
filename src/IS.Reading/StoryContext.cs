using System;
using System.Collections.Generic;

namespace IS.Reading
{
    public class StoryContext : IStoryContextUpdater
    {
        public Dictionary<string, object> Values = new Dictionary<string, object>();

        public event EventHandler<string>? OnMusicChange;
        public event EventHandler<string>? OnBackgroundChange;
        public event EventHandler? OnNarrationOpen;
        public event EventHandler<string>? OnNarrationChange;
        public event EventHandler? OnNarrationClose;
        public event EventHandler? OnTutorialOpen;
        public event EventHandler<string>? OnTutorialChange;
        public event EventHandler? OnTutorialClose;
        public event EventHandler<string>? OnChar1Arrives;
        public event EventHandler<string>? OnChar1Feels;
        public event EventHandler<string>? OnChar1Thinks;
        public event EventHandler<string>? OnChar1Says;
        public event EventHandler<string>? OnChar1Goesaway;
        public event EventHandler<string>? OnChar2Arrives;
        public event EventHandler<string>? OnChar2Feels;
        public event EventHandler<string>? OnChar2Thinks;
        public event EventHandler<string>? OnChar2Says;
        public event EventHandler<string>? OnChar2Goesaway;

        object? IStoryContextUpdater.this[string key] { 
            get {
                if (Values.TryGetValue(key, out var value))
                    return value;
                return null;
            }
            set
            {
                if (value == null)
                {
                    if (Values.ContainsKey(key))
                        Values.Remove(key);
                }
                else
                {
                    Values[key] = value;
                }
            }
        }

        IClosableStoryEvent? IStoryContextUpdater.CurrentEvent { get; set; }

        void IStoryContextUpdater.CallOnBackgroundChange(string name) => OnBackgroundChange?.Invoke(this, name);
        void IStoryContextUpdater.CallOnMusicChange(string name) => OnMusicChange?.Invoke(this, name);
        void IStoryContextUpdater.CallOnNarrationOpen() => OnNarrationOpen?.Invoke(this, new EventArgs());
        void IStoryContextUpdater.CallOnNarrationChange(string name) => OnNarrationChange?.Invoke(this, name);
        void IStoryContextUpdater.CallOnNarrationClose() => OnNarrationClose?.Invoke(this, new EventArgs());
        void IStoryContextUpdater.CallOnTutorialOpen() => OnTutorialOpen?.Invoke(this, new EventArgs());
        void IStoryContextUpdater.CallOnTutorialChange(string name) => OnTutorialChange?.Invoke(this, name);
        void IStoryContextUpdater.CallOnTutorialClose() => OnTutorialClose?.Invoke(this, new EventArgs());

        public void ClosePending() {
            IStoryContextUpdater updater = this;
            updater.CurrentEvent?.Close(this);
        }
    }
}
