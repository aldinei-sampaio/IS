using IS.Reading.StoryboardItems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace IS.Reading.Parsers
{
    public class StoryboardParser
    {
        private const string varNamePattern = @"^[a-z0-9_]+$";

        private readonly Storyboard storyboard;
        private readonly Stack<StoryboardBlock> blocks;
        private readonly XmlReader reader;

        private StoryboardBlock currentBlock;

        private StoryboardParser(XmlReader reader)
        { 
            this.reader = reader;
            storyboard = new Storyboard();
            blocks = new Stack<StoryboardBlock>();
            currentBlock = storyboard.Root;
        }

        public static Storyboard? Load(string content)
        {
            using var reader = XmlReader.Create(new StringReader(content));
            var parser = new StoryboardParser(reader);

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
            switch (reader.LocalName)
            {
                case "viewpoint":
                    CloseBlockIfNecessary();
                    Add(new ProtagonistChangeItem(GetVariableName(), null));
                    break;
                case "background":
                    CloseBlockIfNecessary();
                    Add(new BackgroundItem(GetVariableName(), null));
                    break;
                case "music":
                    CloseBlockIfNecessary();
                    Add(new MusicItem(GetVariableName(), null));
                    break;
                case "unset":
                    CloseBlockIfNecessary();
                    Add(new VarSetItem(GetVariableName(), 0, null));
                    break;
                case "set":
                    CloseBlockIfNecessary();
                    HandleSet(null);
                    break;
                case "observe":
                    CloseBlockIfNecessary();
                    EnsureEmpty();
                    Add(new PauseItem(null));
                    break;
                case "narration":
                    CloseBlockIfNecessary(typeof(NarrationItem));
                    HandleNarration(null);
                    break;
            }
        }

        private void Add(IStoryboardItem item) => currentBlock.ForwardQueue.Enqueue(item);

        private string GetVariableName() => GetContent(varNamePattern);

        private void EnsureEmpty()
        {
            var elementName = reader.LocalName;
            var value = reader.ReadElementContentAsString();
            if (!string.IsNullOrEmpty(value))
                throw new StoryboardParsingException(reader, $"O element '{elementName}' não pode ter conteúdo.");
        }

        private string GetContent(string? pattern = null)
        {
            var elementName = reader.LocalName;
            var value = reader.ReadElementContentAsString();
            if (string.IsNullOrEmpty(value))
                throw new StoryboardParsingException(reader, $"Conteúdo é requerido para o elemento '{elementName}'.");
            if (pattern != null && !Regex.IsMatch(value, pattern))
                throw new StoryboardParsingException(reader, elementName, value);
            return value;
        }              

        private void HandleSet(ICondition? condition)
        {
            var elementName = reader.LocalName;
            var value = GetContent();
            if (Regex.IsMatch(value, VarIncrement.Pattern))
            {
                var increment = VarIncrement.Parse(value);
                if (!increment.HasValue)
                    throw new StoryboardParsingException(reader, elementName, value);
                Add(new VarIncrementItem(increment.Value.Key, increment.Value.Increment, condition));
                return;
            }
            
            if (!Regex.IsMatch(value, varNamePattern))
                throw new StoryboardParsingException(reader, elementName, value);

            Add(new VarSetItem(value, 1, condition));
        }

        private void HandleNarration(ICondition? condition)
        {
            var value = GetContent();
            if (currentBlock.Parent == null || currentBlock.Parent.GetType() != typeof(NarrationItem))
            {
                blocks.Push(currentBlock);
                var narration = new NarrationItem(null);
                Add(narration);
                currentBlock = narration.Block;
            }
            Add(new NarrationTextItem(value, condition));
        }

        private void CloseBlockIfNecessary(Type? ignore = null)
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
                currentBlock = blocks.Pop();
            }
        }

        private void HandleEndElement()
        {
        }

        private (string ElementName, int LineNumber, int LinePosition) GetInfo()
        {
            var info = (IXmlLineInfo)reader;
            return (reader.LocalName, info.LineNumber, info.LinePosition);
        }
    }

    public class StoryboardParsingException : Exception
    {
        internal StoryboardParsingException(XmlReader reader, string message) 
            : base(GetMessage(reader, message))
        {
        }

        private static string GetMessage(XmlReader reader, string message)
        {
            var info = (IXmlLineInfo)reader;
            return $"{message}\r\nLinha {info.LineNumber}";
        }

        internal StoryboardParsingException(XmlReader reader, string elementName, string value)
            : base(GetMessage(reader, elementName, value))
        {
        }

        private static string GetMessage(XmlReader reader, string elementName, string value)
        {
            var info = (IXmlLineInfo)reader;
            return $"O valor '{value}' não é válido para o elemento '{elementName}'.\r\nLinha {info.LineNumber}";
        }
    }
}
