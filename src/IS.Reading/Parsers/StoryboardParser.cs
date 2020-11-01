using IS.Reading.Conditions;
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
        private const string varNamePattern = @"^[a-z0-9_]{2,}$";

        private readonly Storyboard storyboard;
        private readonly Stack<StoryboardBlock> blocks;
        private readonly XmlReader reader;

        private StoryboardBlock currentBlock;
        private PromptItem? currentPrompt;

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

            reader.MoveToContent();

            if (reader.LocalName != "storyboard")
                throw new StoryboardParsingException(reader, "Elemento 'storyboard' não encontrado.");

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
            if (Is<ProtagonistSpeechItem>(currentBlock))
                if (HandleProtagonistSpeechStartElement())
                    return;

            if (Is<ProtagonistThoughtItem>(currentBlock))
                if (HandleProtagonistThoughtStartElement())
                    return;

            if (Is<ProtagonistMoodItem>(currentBlock))
                if (HandleProtagonistStartElement(true))
                    return;

            if (Is<ProtagonistItem>(currentBlock))
                if (HandleProtagonistStartElement(false))
                    return;

            switch (reader.LocalName)
            {
                case "viewpoint":
                    {
                        CloseBlockIfNecessary();
                        var condition = LookForCondition();
                        Add(new ProtagonistChangeItem(GetVariableName(), condition));
                        break;
                    }
                case "background":
                    {
                        CloseBlockIfNecessary();
                        var condition = LookForCondition();
                        Add(new BackgroundItem(GetVariableName(), condition));
                        break;
                    }
                case "music":
                    {
                        CloseBlockIfNecessary();
                        var condition = LookForCondition();
                        Add(new MusicItem(GetVariableName(), condition));
                        break;
                    }
                case "unset":
                    {
                        CloseBlockIfNecessary();
                        var condition = LookForCondition();
                        Add(new VarSetItem(GetVariableName(), 0, condition));
                        break;
                    }
                case "set":
                    CloseBlockIfNecessary();
                    HandleSet();
                    break;
                case "observe":
                    CloseBlockIfNecessary();
                    EnsureEmpty();
                    Add(new PauseItem(LookForCondition()));
                    break;
                case "narration":
                    {
                        var condition = LookForCondition();
                        var value = GetContent();
                        OpenBlockIfNecessary(() => new NarrationItem(null));
                        Add(new NarrationTextItem(value, condition));
                        break;
                    }
                case "tutorial":
                    {
                        var condition = LookForCondition();
                        var value = GetContent();
                        OpenBlockIfNecessary(() => new TutorialItem(null));
                        Add(new TutorialTextItem(value, condition));
                        break;
                    }
                case "protagonist":
                    {
                        CloseBlockIfNecessary();
                        EnsureEmpty();
                        if (LookForCondition() != null)
                            throw new StoryboardParsingException(reader, "O elemento 'protagonist' não suporta condições 'when'.");
                        OpenBlock(new ProtagonistItem(null));
                        break;
                    }
                case "prompt":
                    HandlePrompt();
                    break;
                default:
                    throw new StoryboardParsingException(reader, $"O elemento '{reader.LocalName}' não é suportado.");
            }
        }

        private static bool Is<T>(StoryboardBlock block)
            => block.Parent?.GetType() == typeof(T);

        private void CloseToRootOrDo()
        {
            while (currentBlock.Parent != null)
                CloseBlock();
        }


        private bool HandleProtagonistStartElement(bool isMood)
        {
            switch (reader.LocalName)
            {
                case "emotion":
                    {
                        if (isMood)
                            CloseBlock();
                        var condition = LookForCondition();
                        OpenBlock(new ProtagonistMoodItem(GetVariableName(), condition));
                        return true;
                    }
                case "voice":
                    {
                        var condition = LookForCondition();
                        OpenBlock(new ProtagonistSpeechItem(null));
                        Add(new ProtagonistSpeechTextItem(GetContent(), condition));
                        return true;
                    }
                case "thought":
                    {
                        var condition = LookForCondition();
                        OpenBlock(new ProtagonistThoughtItem(null));
                        Add(new ProtagonistThoughtTextItem(GetContent(), condition));
                        return true;
                    }
                case "prompt":
                    HandlePrompt();
                    return true;
                case "reward":
                    return true;
                case "set":
                    HandleSet();
                    return true;
                case "unset":
                    {
                        var condition = LookForCondition();
                        Add(new VarSetItem(GetVariableName(), 0, condition));
                        return true;
                    }
                case "bump":
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
                case "voice":
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
                case "thought":
                    var condition = LookForCondition();
                    Add(new ProtagonistThoughtTextItem(GetContent(), condition));
                    return true;
            }
            CloseBlock();
            return false;
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
                Add(new VarIncrementItem(increment.Value.Key, increment.Value.Increment, condition));
                return;
            }
            
            if (!Regex.IsMatch(value, varNamePattern))
                throw new StoryboardParsingException(reader, elementName, value);

            Add(new VarSetItem(value, 1, condition));
        }

        private void HandlePrompt()
        {
            ICondition? condition = null;
            TimeSpan? timeLimit = null;
            string? defaultChoice = null;
            var randomOrder = false;

            if (reader.IsEmptyElement)
                throw new StoryboardParsingException(reader, $"O elemento 'prompt' não pode estar vazio.");

            while (reader.MoveToNextAttribute())
            {
                switch (reader.LocalName)
                {
                    case "when":
                        condition = GetCondition(reader.Value);
                        break;
                    case "time":
                        if (!int.TryParse(reader.Value, out var seconds))
                            throw new StoryboardParsingException(reader, $"O valor '{reader.Value}' não é válido para o atributo 'time'. É esperado um número inteiro.");
                        timeLimit = TimeSpan.FromSeconds(seconds);
                        break;
                    case "default":
                        if (!Regex.IsMatch(reader.Value, @"^[a-z]$"))
                            throw new StoryboardParsingException(reader, $"O valor '{reader.Value}' não é válido para o atributo 'default'. É esperada uma opção de 'a' a 'z'.");
                        defaultChoice = reader.Value;
                        break;
                    case "randomorder":
                        switch (reader.Value)
                        {
                            case "true":
                            case "1":
                                randomOrder = true;
                                break;
                            case "false":
                            case "0":
                                randomOrder = false;
                                break;
                            default:
                                throw new StoryboardParsingException(reader, $"O valor '{reader.Value}' não é válido para o atributo 'randomorder'. É esperado '1' ou '0'.");
                        }
                        break;
                    default:
                        throw new StoryboardParsingException(reader, $"O atributo '{reader.LocalName}' não é suportado para o elemento 'choice'.");
                }
            }

            var choices = new List<Choice>();
            var triggerFound = false;
            IStoryboardItem trigger = null;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (Regex.IsMatch(reader.LocalName, @"^[a-z]$"))
                    {
                        choices.Add(LoadChoice());
                    }
                    else
                    {
                        trigger = HandlePromptTriggerElement(triggerFound);
                        triggerFound = true;
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (reader.LocalName == "prompt")
                    {
                        if (choices.Count == 0)
                            throw new StoryboardParsingException(reader, $"Nenhuma escolha definida no elemento 'prompt'. Favor definir um ou mais elementos de 'a' a 'z'.");
                        break;
                    }
                }
            }

            if (trigger == null)
                throw new StoryboardParsingException(reader, $"O elemento 'prompt' precisa conter um elemento 'narration', 'tutorial', 'voice' ou 'thought'.");

            var prompt = new Prompt(choices, timeLimit, defaultChoice, randomOrder);
            var promptItem = new PromptItem(prompt, condition);
            promptItem.Block.ForwardQueue.Enqueue(trigger);
            Add(promptItem);
        }

        private IStoryboardItem HandlePromptTriggerElement(bool triggerFound)
        {
            void Validate()
            {
                var elementName = reader.LocalName;
                if (triggerFound)
                    throw new StoryboardParsingException(reader, $"O elemento '{elementName}' é inválido porque existe outro elemento de pausa dentro do 'prompt'.");
                if (reader.MoveToNextAttribute())
                    throw new StoryboardParsingException(reader, $"Não são permitidos atributos no elemento '{elementName}' dentro de um elemento 'prompt'.");
            }

            void CloseToProtagonist()
            {
                for(; ; )
                {
                    if (Is<ProtagonistItem>(currentBlock))
                        return;

                    if (blocks.Count == 0)
                        throw new StoryboardParsingException(reader, $"O elemento '{reader.LocalName}' só pode ser usado em um 'prompt' quando o mesmo estiver ligado a um 'protagonist'.");

                    CloseBlock();
                }
            }

            switch (reader.LocalName)
            {
                case "narration":
                    {
                        CloseToRootOrDo();
                        Validate();
                        var item = new NarrationItem(null);
                        item.Block.ForwardQueue.Enqueue(new NarrationTextItem(GetContent(), null));
                        return item;
                    }
                case "tutorial":
                    {
                        CloseToRootOrDo();
                        Validate();
                        var item = new TutorialItem(null);
                        item.Block.ForwardQueue.Enqueue(new TutorialTextItem(GetContent(), null));
                        return item;
                    }
                case "voice":
                    {
                        CloseToProtagonist();
                        Validate();
                        var item = new ProtagonistSpeechItem(null);
                        item.Block.ForwardQueue.Enqueue(new ProtagonistSpeechTextItem(GetContent(), null));
                        return item;
                    }
                case "thought":
                    {
                        CloseToProtagonist();
                        Validate();
                        var item = new ProtagonistThoughtItem(null);
                        item.Block.ForwardQueue.Enqueue(new ProtagonistThoughtTextItem(GetContent(), null));
                        return item;
                    }
                case "observe":
                    CloseToRootOrDo();
                    Validate();
                    return new PauseItem(null);
                default:
                    throw new StoryboardParsingException(reader, $"O elemento '{reader.LocalName}' não é suportado dentro de um elemento 'prompt'.");
            }
        }

        private Choice LoadChoice()
        {
            var value = reader.LocalName;

            if (reader.IsEmptyElement)
                throw new StoryboardParsingException(reader, $"O elemento '{value}' precisa possuir conteúdo.");

            ICondition? condition = null;
            var tip = string.Empty;
            while (reader.MoveToNextAttribute())
            {
                if (reader.LocalName == "when")
                    condition = ConditionParser.Parse(reader.Value);
                else if (reader.LocalName == "req")
                    tip = reader.Value;
                else
                    throw new StoryboardParsingException(reader, $"O atributo '{reader.LocalName}' não é suportado para o elemento '{value}'.");
            }

            var text = string.Empty;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Text)
                    text = reader.Value;
                else if (reader.NodeType == XmlNodeType.EndElement)
                    break;
                else if (reader.NodeType == XmlNodeType.Element)
                    throw new StoryboardParsingException(reader, $"O elemento '{reader.LocalName}' não é suportado como filho do elemento '{value}'.");
            }

            if (string.IsNullOrEmpty(text))
                throw new StoryboardParsingException(reader, $"O elemento '{value}' precisa possuir conteúdo.");

            return new Choice(value, text, tip, condition);
        }

        private ICondition? LookForCondition()
        {
            var elementName = reader.LocalName;
            ICondition? condition = null;

            if (reader.MoveToNextAttribute())
            {
                if (reader.LocalName == "when")
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
                throw new StoryboardParsingException(reader, $"O valor '{text}' não é válido para o atributo 'when'.");

            return condition;
        }

        private void OpenBlockIfNecessary<T>(Func<T> creator) where T : IStoryboardItem
        {
            CloseBlockIfNecessary(typeof(T));
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
                CloseBlock();
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
