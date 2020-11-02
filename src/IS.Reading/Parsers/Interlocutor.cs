using IS.Reading.StoryboardItems;

namespace IS.Reading.Parsers
{
    public partial class StoryboardParser
    {
        private bool HandleInterlocutorStartElement(bool isMood)
        {
            switch (reader.LocalName)
            {
                case Emotion:
                    {
                        if (isMood)
                            CloseBlock();
                        var condition = LookForCondition();
                        OpenBlock(new InterlocutorMoodItem(GetVariableName(), condition));
                        return true;
                    }
                case Speech:
                    {
                        var condition = LookForCondition();
                        OpenBlock(new InterlocutorSpeechItem(null));
                        Add(new InterlocutorSpeechTextItem(GetContent(), condition));
                        return true;
                    }
                case Thought:
                    {
                        var condition = LookForCondition();
                        OpenBlock(new InterlocutorThoughtItem(null));
                        Add(new InterlocutorThoughtTextItem(GetContent(), condition));
                        return true;
                    }
                case Reward:
                    HandleInterlocutorReward();
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
                    HandleInterlocutorBump();
                    return true;
            }
            CloseBlock();
            return false;
        }

        private bool HandleInterlocutorBumpStartElement()
        {
            switch (reader.LocalName)
            {
                case Speech:
                    {
                        if (LookForCondition() != null)
                            throw new StoryboardParsingException(reader, $"O elemento após '{Bump}' não pode ter condição '{When}'.");
                        if (!Is<InterlocutorSpeechItem>())
                            OpenBlock(new InterlocutorSpeechItem(null));
                        OpenBlock(new InterlocutorBumpItem(null));
                        Add(new InterlocutorSpeechTextItem(GetContent(), null));
                        CloseBlock();
                        isInterlocutorBump = false;
                        return true;
                    }
                case Thought:
                    {
                        if (LookForCondition() != null)
                            throw new StoryboardParsingException(reader, $"O elemento após '{Bump}' não pode ter condição '{When}'.");
                        if (!Is<InterlocutorThoughtItem>())
                            OpenBlock(new InterlocutorThoughtItem(null));
                        OpenBlock(new InterlocutorBumpItem(null));
                        Add(new InterlocutorThoughtTextItem(GetContent(), null));
                        CloseBlock();
                        isInterlocutorBump = false;
                        return true;
                    }
                default:
                    throw new StoryboardParsingException(reader, $"O elemento '{reader.LocalName}' não pode vir depois do elemento '{Bump}'. É esperado '{Thought}' ou '{Speech}'.");
            }
        }

        private bool HandleInterlocutorSpeechStartElement()
        {
            switch (reader.LocalName)
            {
                case Speech:
                    var condition = LookForCondition();
                    Add(new InterlocutorSpeechTextItem(GetContent(), condition));
                    return true;
                case Reward:
                    HandleInterlocutorReward();
                    return true;
                case Bump:
                    HandleInterlocutorBump();
                    return true;
            }
            CloseBlock();
            return false;
        }

        private bool HandleInterlocutorThoughtStartElement()
        {
            switch (reader.LocalName)
            {
                case Thought:
                    var condition = LookForCondition();
                    Add(new InterlocutorThoughtTextItem(GetContent(), condition));
                    return true;
                case Reward:
                    HandleInterlocutorReward();
                    return true;
                case Bump:
                    HandleInterlocutorBump();
                    return true;
            }
            CloseBlock();
            return false;
        }

        private void HandleInterlocutorBump()
        {
            EnsureEmpty();
            if (LookForCondition() != null)
                throw new StoryboardParsingException(reader, $"O elemento '{Bump}' não pode ter condição '{When}'.");
            isInterlocutorBump = true;
        }

        private void HandleInterlocutorReward()
        {
            var condition = LookForCondition();
            var increment = LoadIncrement();
            Add(new InterlocutorRewardItem(increment, condition));
        }


    }
}
