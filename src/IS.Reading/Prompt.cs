using System;
using System.Collections.Generic;

namespace IS.Reading
{
    public struct Prompt
    {
        public IReadOnlyList<Choice> Choices { get; }
        public TimeSpan? TimeLimit { get; }
        public string? DefaultChoice { get; }

        public Prompt(IReadOnlyList<Choice> choices, TimeSpan? timeLimit, string? defaultChoice)
        {
            Choices = choices;
            TimeLimit = timeLimit;
            DefaultChoice = defaultChoice;
        }
    }
}
