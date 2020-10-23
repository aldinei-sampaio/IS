namespace IS.Reading
{
    public interface IStoryContextUpdater
    {
        string this[string key] { get; set; }
        string SetValue(string key, string newValue);

        void CallOnBackgroundChange(string name);
        void CallOnMusicChange(string name);
        void CallOnNarrationOpen();
        void CallOnNarrationChange(string text);
        void CallOnNarrationClose();
        void CallOnTutorialOpen();
        void CallOnTutorialChange(string text);
        void CallOnTutorialClose();
        void CallOnProtagonistChange(string name);
        void CallOnProtagonistArrive(string text);
        void CallOnProtagonistFeelingChange(string text);
        void CallOnProtagonistSpeakOpen();
        void CallOnProtagonistSpeakChange(string text);
        void CallOnProtagonistSpeakClose();
        void CallOnProtagonistThoughtOpen();
        void CallOnProtagonistThoughtChange(string text);
        void CallOnProtagonistThoughtClose();
        void CallOnProtagonistLeave();
        void CallOnPromptOpen(Prompt prompt);
    }
}
