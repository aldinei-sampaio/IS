using IS.Reading.Conditions;
using IS.Reading.StoryboardItems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace IS.Reading.Parsers
{
    public partial class StoryboardParser
    {
        private const string varNamePattern = /* language=regex */ @"^[a-z0-9_]{2,}$";
        private readonly Storyboard storyboard;
        private readonly Stack<StoryboardBlock> blocks;
        private readonly XmlReader reader;

        private StoryboardBlock currentBlock;
        private bool isProtagonistBump;
        private bool isInterlocutorBump;

        private StoryboardParser(XmlReader reader)
        { 
            this.reader = reader;
            storyboard = new Storyboard();
            blocks = new Stack<StoryboardBlock>();
            currentBlock = storyboard.Root;
        }

        public static Storyboard Parse(Stream stream)
            => Parse(new StreamReader(stream));

        public static Storyboard Parse(string content) 
            => Parse(new StringReader(content));        

        public static Storyboard Parse(TextReader textReader)
        {
            using var reader = XmlReader.Create(textReader);
            var parser = new StoryboardParser(reader);

            reader.MoveToContent();

            if (reader.LocalName != Storyboard)
                throw new StoryboardParsingException(reader, $"Elemento '{Storyboard}' não encontrado.");

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                    parser.HandleStartElement();
                else if (reader.NodeType == XmlNodeType.EndElement)
                    parser.HandleEndElement();
            }

            return parser.storyboard;
        }

        private void HandleStartElement()
        {
            if (isInterlocutorBump)
                if (HandleInterlocutorBumpStartElement())
                    return;

            if (Is<InterlocutorSpeechItem>())
                if (HandleInterlocutorSpeechStartElement())
                    return;

            if (Is<InterlocutorThoughtItem>())
                if (HandleInterlocutorThoughtStartElement())
                    return;

            if (Is<InterlocutorMoodItem>())
                if (HandleInterlocutorStartElement(true))
                    return;

            if (Is<InterlocutorItem>())
                if (HandleInterlocutorStartElement(false))
                    return;

            if (isProtagonistBump)
                if (HandleProtagonistBumpStartElement())
                    return;

            if (Is<ProtagonistSpeechItem>())
                if (HandleProtagonistSpeechStartElement())
                    return;

            if (Is<ProtagonistThoughtItem>())
                if (HandleProtagonistThoughtStartElement())
                    return;

            if (Is<ProtagonistMoodItem>())
                if (HandleProtagonistStartElement(true))
                    return;

            if (Is<ProtagonistItem>())
                if (HandleProtagonistStartElement(false))
                    return;

            switch (reader.LocalName)
            {
                case ViewPoint:
                    {
                        CloseTalkBlockIfNecessary();
                        var condition = LookForCondition();
                        Add(new ProtagonistChangeItem(GetVariableName(), condition));
                        break;
                    }
                case Background:
                    {
                        CloseTalkBlockIfNecessary();
                        var condition = LookForCondition();
                        Add(new BackgroundItem(GetVariableName(), condition));
                        break;
                    }
                case Music:
                    {
                        CloseTalkBlockIfNecessary();
                        var condition = LookForCondition();
                        Add(new MusicItem(GetVariableName(), condition));
                        break;
                    }
                case Unset:
                    {
                        CloseTalkBlockIfNecessary();
                        var condition = LookForCondition();
                        Add(new VarSetItem(GetVariableName(), 0, condition));
                        break;
                    }
                case Set:
                    CloseTalkBlockIfNecessary();
                    HandleSet();
                    break;
                case Observe:
                    CloseTalkBlockIfNecessary();
                    EnsureEmpty();
                    Add(new PauseItem(LookForCondition()));
                    break;
                case Narration:
                    {
                        var condition = LookForCondition();
                        var value = GetContent();
                        OpenBlockIfNecessary(() => new NarrationItem(null));
                        Add(new NarrationTextItem(value, condition));
                        break;
                    }
                case Tutorial:
                    {
                        var condition = LookForCondition();
                        var value = GetContent();
                        OpenBlockIfNecessary(() => new TutorialItem(null));
                        Add(new TutorialTextItem(value, condition));
                        break;
                    }
                case Protagonist:
                    {
                        CloseTalkBlockIfNecessary();
                        EnsureEmpty();
                        if (LookForCondition() != null)
                            throw new StoryboardParsingException(reader, $"O elemento '{Protagonist}' não suporta condições '{When}'.");
                        OpenBlock(new ProtagonistItem(null));
                        break;
                    }
                case Interlocutor:
                    {
                        CloseTalkBlockIfNecessary();
                        if (LookForCondition() != null)
                            throw new StoryboardParsingException(reader, $"O elemento '{Interlocutor}' não suporta condições '{When}'.");
                        OpenBlock(new InterlocutorItem(GetContent(), null));
                        break;
                    }
                case Prompt:
                    HandlePrompt();
                    break;
                case Do:
                    {
                        var condition = LookForCondition();
                        OpenBlock(new ConditionItem(condition));
                        break;
                    }
                default:
                    throw new StoryboardParsingException(reader, $"O elemento '{reader.LocalName}' não é suportado.");
            }
        }

        private bool Is<T>()
            => currentBlock.Parent?.GetType() == typeof(T);

        private void CloseToRootOrDo()
        {
            while (currentBlock.Parent != null && !Is<ConditionItem>())
                CloseBlock();
        }

        private void Add(IStoryboardItem item) => currentBlock.ForwardQueue.Enqueue(item);

        private string GetVariableName() => GetContent(varNamePattern);

        private void EnsureEmpty()
        {
            var elementName = reader.LocalName;
            if (!reader.IsEmptyElement)
                throw new StoryboardParsingException(reader, $"O elemento '{elementName}' não pode ter conteúdo.");
        }

        private string GetContent(string? pattern = null)
        {
            var elementName = reader.LocalName;
            reader.Read();
            if (reader.NodeType != XmlNodeType.Text)
                throw new StoryboardParsingException(reader, $"Conteúdo é requerido para o elemento '{elementName}'.");
            var value = reader.ReadContentAsString();
            if (string.IsNullOrEmpty(value))
                throw new StoryboardParsingException(reader, $"Conteúdo é requerido para o elemento '{elementName}'.");
            if (pattern != null && !Regex.IsMatch(value, pattern))
                throw new StoryboardParsingException(reader, elementName, value);
            return value;
        }              

        private void HandleSet()
        {
            var elementName = reader.LocalName;
            var condition = LookForCondition();
            var value = GetContent();
            if (Regex.IsMatch(value, VarIncrement.Pattern))
            {
                var increment = VarIncrement.Parse(value);
                if (!increment.HasValue)
                    throw new StoryboardParsingException(reader, elementName, value);
                Add(new VarIncrementItem(increment.Value, condition));
                return;
            }
            
            if (!Regex.IsMatch(value, varNamePattern))
                throw new StoryboardParsingException(reader, elementName, value);

            Add(new VarSetItem(value, 1, condition));
        }

        private VarIncrement LoadIncrement()
        {
            var elementName = reader.LocalName;
            var value = GetContent();
            if (!Regex.IsMatch(value, VarIncrement.Pattern))
                throw new StoryboardParsingException(reader, elementName, value);

            var increment = VarIncrement.Parse(value);
            if (!increment.HasValue)
                throw new StoryboardParsingException(reader, elementName, value);

            return increment.Value;
        }

        private ICondition? LookForCondition()
        {
            var elementName = reader.LocalName;
            ICondition? condition = null;

            if (reader.MoveToNextAttribute())
            {
                if (reader.LocalName == When)
                    condition = GetCondition(reader.Value);
                else
                    throw new StoryboardParsingException(reader, $"O atributo '{reader.LocalName}' não é suportado para o elemento '{elementName}'.");

                if (reader.MoveToNextAttribute())
                    throw new StoryboardParsingException(reader, $"O atributo '{reader.LocalName}' não é suportado para o elemento '{elementName}'.");
            }

            return condition;
        }

        private ICondition? GetCondition(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            var condition = ConditionParser.Parse(text);
            if (condition == null)
                throw new StoryboardParsingException(reader, $"O valor '{text}' não é válido para o atributo '{When}'.");

            return condition;
        }

        private void OpenBlockIfNecessary<T>(Func<T> creator) where T : IStoryboardItem
        {
            CloseTalkBlockIfNecessary(typeof(T));
            if (currentBlock.Parent == null || currentBlock.Parent.GetType() != typeof(T))
                OpenBlock(creator.Invoke());
        }

        private void OpenBlock(IStoryboardItem item)
        {
            blocks.Push(currentBlock);
            if (item.Block == null)
                throw new InvalidOperationException();
            Add(item);
            currentBlock = item.Block;
        }

        private void CloseBlock() => currentBlock = blocks.Pop();

        private void CloseTalkBlockIfNecessary(Type? ignore = null)
        {
            if (currentBlock.Parent == null)
                return;

            var type = currentBlock.Parent.GetType();

            if (ignore != null && type == ignore)
                return;

            if (type == typeof(NarrationItem) 
                || type == typeof(TutorialItem) 
                || type == typeof(InterlocutorSpeechItem)
                || type == typeof(ProtagonistSpeechItem)
                || type == typeof(InterlocutorThoughtItem)
                || type == typeof(ProtagonistThoughtItem))
            {
                CloseBlock();
            }
        }

        private void HandleEndElement()
        {
            switch (reader.LocalName)
            {
                case Do:
                    while (!Is<ConditionItem>())
                        CloseBlock();
                    CloseBlock();
                    break;
            }
        }
    }
}
