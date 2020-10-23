using System;
using System.Collections.Generic;
using System.Text;

namespace IS.Reading
{
    public interface IStoryContextEvents
    {
        event EventHandler<string>? OnMusicChange;
        event EventHandler<string>? OnBackgroundChange;
        event EventHandler? OnNarrationOpen;
        event EventHandler<string>? OnNarrationChange;
        event EventHandler? OnNarrationClose;
        event EventHandler? OnTutorialOpen;
        event EventHandler<string>? OnTutorialChange;
        event EventHandler? OnTutorialClose;

        event EventHandler<string>? OnProtagonistChange;
        event EventHandler<string>? OnProtagonistArrive;
        event EventHandler<string>? OnProtagonistFeelingChange;
        event EventHandler? OnProtagonistThoughtOpen;
        event EventHandler<string>? OnProtagonistThoughtChange;
        event EventHandler? OnProtagonistThoughtClose;
        event EventHandler? OnProtagonistSpeakOpen;
        event EventHandler<string>? OnProtagonistSpeakChange;
        event EventHandler? OnProtagonistSpeakClose;
        event EventHandler? OnProtagonistLeave;

        event EventHandler<Prompt>? OnPromptOpen;
    }
}
