using System;

namespace IS.Reading
{
    public interface IBookReader
    {
        void LoadData(string data);
        bool Read();
        event EventHandler<string> OnMusicChange;
        event EventHandler<string> OnBackgroundChange;
        event EventHandler<string> OnNarration;
        event EventHandler<string> OnChar1Arrives;
        event EventHandler<string> OnChar1Feels;
        event EventHandler<string> OnChar1Thinks;
        event EventHandler<string> OnChar1Says;
        event EventHandler<string> OnChar1Goesaway;
        event EventHandler<string> OnChar2Arrives;
        event EventHandler<string> OnChar2Feels;
        event EventHandler<string> OnChar2Thinks;
        event EventHandler<string> OnChar2Says;
        event EventHandler<string> OnChar2Goesaway;

    }
}
