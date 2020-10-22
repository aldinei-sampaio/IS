namespace IS.Reading
{
    public interface IStoryContextUpdater
    {
        object? this[string key] { get; set; }
        IClosableStoryEvent? CurrentEvent { get; set; }

        void CallOnBackgroundChange(string name);
        void CallOnMusicChange(string name);
        void CallOnNarrationOpen();
        void CallOnNarrationChange(string text);
        void CallOnNarrationClose();
        void CallOnTutorialOpen();
        void CallOnTutorialChange(string text);
        void CallOnTutorialClose();
    }
}
