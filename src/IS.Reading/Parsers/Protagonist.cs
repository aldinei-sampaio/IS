using IS.Reading.StoryboardItems;

namespace IS.Reading.Parsers
{
    public partial class StoryboardParser
    {
        private bool HandleProtagonistStartElement(bool isMood)
        {
            switch (reader.LocalName)
            {
                case Emotion:
                    {
                        if (isMood)
                            CloseBlock();
                        var condition = LookForCondition();
                        OpenBlock(new ProtagonistMoodItem(GetVariableName(), condition));
                        return true;
                    }
                case Speech:
                    {
                        var condition = LookForCondition();
                        OpenBlock(new ProtagonistSpeechItem(null));
                        Add(new ProtagonistSpeechTextItem(GetContent(), condition));
                        return true;
                    }
                case Thought:
                    {
                        var condition = LookForCondition();
                        OpenBlock(new ProtagonistThoughtItem(null));
                        Add(new ProtagonistThoughtTextItem(GetContent(), condition));
                        return true;
                    }
                case Prompt:
                    HandlePrompt();
                    return true;
                case Reward:
                    return true;
                case Set:
                    HandleSet();
                    return true;
                case Unset:
                    {
                        var condition = LookForCondition();
                        Add(new VarSetItem(GetVariableName(), 0, condition));
                        return true;
                    }
                case Bump:
                    EnsureEmpty();
                    Add(new ProtagonistBumpItem(LookForCondition()));
                    return true;
            }
            CloseBlock();
            return false;
        }

        private bool HandleProtagonistSpeechStartElement()
        {
            switch (reader.LocalName)
            {
                case Speech:
                    var condition = LookForCondition();
                    Add(new ProtagonistSpeechTextItem(GetContent(), condition));
                    return true;
            }
            CloseBlock();
            return false;
        }

        private bool HandleProtagonistThoughtStartElement()
        {
            switch (reader.LocalName)
            {
                case Thought:
                    var condition = LookForCondition();
                    Add(new ProtagonistThoughtTextItem(GetContent(), condition));
                    return true;
            }
            CloseBlock();
            return false;
        }

    }
}
