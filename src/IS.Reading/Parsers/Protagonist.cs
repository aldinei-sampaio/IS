﻿using IS.Reading.StoryboardItems;

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
                    HandleProtagonistReward();
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
                    HandleProtagonistBump();
                    return true;
                case Trophy:
                    HandleTrophy();
                    return true;
            }
            CloseBlock();
            return false;
        }

        private void HandleProtagonistBump()
        {
            EnsureEmpty();
            if (LookForCondition() != null)
                throw new StoryboardParsingException(reader, $"O elemento '{Bump}' não pode ter condição '{When}'.");
            isProtagonistBump = true;
        }

        private void HandleProtagonistReward()
        {
            var condition = LookForCondition();
            var increment = LoadIncrement();
            Add(new ProtagonistRewardItem(increment, condition));
        }

        private bool HandleProtagonistBumpStartElement()
        {
            switch (reader.LocalName)
            {
                case Speech:
                    {
                        if (LookForCondition() != null)
                            throw new StoryboardParsingException(reader, $"O elemento após '{Bump}' não pode ter condição '{When}'.");
                        if (!Is<ProtagonistSpeechItem>())
                            OpenBlock(new ProtagonistSpeechItem(null));
                        OpenBlock(new ProtagonistBumpItem(null));
                        Add(new ProtagonistSpeechTextItem(GetContent(), null));
                        CloseBlock();
                        isProtagonistBump = false;
                        return true;
                    }
                case Thought:
                    {
                        if (LookForCondition() != null)
                            throw new StoryboardParsingException(reader, $"O elemento após '{Bump}' não pode ter condição '{When}'.");
                        if (!Is<ProtagonistThoughtItem>())
                            OpenBlock(new ProtagonistThoughtItem(null));
                        OpenBlock(new ProtagonistBumpItem(null));
                        Add(new ProtagonistThoughtTextItem(GetContent(), null));
                        CloseBlock();
                        isProtagonistBump = false;
                        return true;
                    }
                default:
                    throw new StoryboardParsingException(reader, $"O elemento '{reader.LocalName}' não pode vir depois do elemento '{Bump}'. É esperado '{Thought}' ou '{Speech}'.");
            }
        }

        private bool HandleProtagonistSpeechStartElement()
        {
            switch (reader.LocalName)
            {
                case Speech:
                    var condition = LookForCondition();
                    Add(new ProtagonistSpeechTextItem(GetContent(), condition));
                    return true;
                case Reward:
                    HandleProtagonistReward();
                    return true;
                case Bump:
                    HandleProtagonistBump();
                    return true;
                case Trophy:
                    HandleTrophy();
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
                case Reward:
                    HandleProtagonistReward();
                    return true;
                case Bump:
                    HandleProtagonistBump();
                    return true;
                case Trophy:
                    HandleTrophy();
                    return true;
            }
            CloseBlock();
            return false;
        }
    }
}
