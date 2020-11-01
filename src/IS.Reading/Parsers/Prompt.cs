using IS.Reading.Conditions;
using IS.Reading.StoryboardItems;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace IS.Reading.Parsers
{
    public partial class StoryboardParser
    {
        private void HandlePrompt()
        {
            ICondition? condition = null;
            TimeSpan? timeLimit = null;
            string? defaultChoice = null;
            var randomOrder = false;

            if (reader.IsEmptyElement)
                throw new StoryboardParsingException(reader, $"O elemento '{Prompt}' não pode estar vazio.");

            while (reader.MoveToNextAttribute())
            {
                switch (reader.LocalName)
                {
                    case When:
                        condition = GetCondition(reader.Value);
                        break;
                    case Time:
                        if (!int.TryParse(reader.Value, out var seconds))
                            throw new StoryboardParsingException(reader, $"O valor '{reader.Value}' não é válido para o atributo '{Time}'. É esperado um número inteiro.");
                        timeLimit = TimeSpan.FromSeconds(seconds);
                        break;
                    case Default:
                        if (!Regex.IsMatch(reader.Value, @"^[a-z]$"))
                            throw new StoryboardParsingException(reader, $"O valor '{reader.Value}' não é válido para o atributo '{Default}'. É esperada uma opção de 'a' a 'z'.");
                        defaultChoice = reader.Value;
                        break;
                    case RandomOrder:
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
                                throw new StoryboardParsingException(reader, $"O valor '{reader.Value}' não é válido para o atributo '{RandomOrder}'. É esperado '1' ou '0'.");
                        }
                        break;
                    default:
                        throw new StoryboardParsingException(reader, $"O atributo '{reader.LocalName}' não é suportado para o elemento '{Prompt}'.");
                }
            }

            var choices = new List<Choice>();
            var triggerFound = false;
            IStoryboardItem? trigger = null;

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
                    if (reader.LocalName == Prompt)
                    {
                        if (choices.Count == 0)
                            throw new StoryboardParsingException(reader, $"Nenhuma escolha definida no elemento '{Prompt}'. Favor definir um ou mais elementos de 'a' a 'z'.");
                        break;
                    }
                }
            }

            if (trigger == null)
                throw new StoryboardParsingException(reader, $"O elemento '{Prompt}' precisa conter um elemento '{Narration}', '{Tutorial}', '{Observe}', '{Speech}' ou '{Thought}'.");

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
                    throw new StoryboardParsingException(reader, $"O elemento '{elementName}' é inválido porque existe outro elemento de pausa dentro do '{Prompt}'.");
                if (reader.MoveToNextAttribute())
                    throw new StoryboardParsingException(reader, $"Não são permitidos atributos no elemento '{elementName}' dentro do elemento '{Prompt}'.");
            }

            void CloseToProtagonist()
            {
                for (; ; )
                {
                    if (Is<ProtagonistItem>())
                        return;

                    if (blocks.Count == 0)
                        throw new StoryboardParsingException(reader, $"O elemento '{reader.LocalName}' só pode ser usado em um '{Prompt}' quando o mesmo estiver ligado a um '{Protagonist}'.");

                    CloseBlock();
                }
            }

            switch (reader.LocalName)
            {
                case Narration:
                    {
                        CloseToRootOrDo();
                        Validate();
                        var item = new NarrationItem(null);
                        item.Block.ForwardQueue.Enqueue(new NarrationTextItem(GetContent(), null));
                        return item;
                    }
                case Tutorial:
                    {
                        CloseToRootOrDo();
                        Validate();
                        var item = new TutorialItem(null);
                        item.Block.ForwardQueue.Enqueue(new TutorialTextItem(GetContent(), null));
                        return item;
                    }
                case Speech:
                    {
                        CloseToProtagonist();
                        Validate();
                        var item = new ProtagonistSpeechItem(null);
                        item.Block.ForwardQueue.Enqueue(new ProtagonistSpeechTextItem(GetContent(), null));
                        return item;
                    }
                case Thought:
                    {
                        CloseToProtagonist();
                        Validate();
                        var item = new ProtagonistThoughtItem(null);
                        item.Block.ForwardQueue.Enqueue(new ProtagonistThoughtTextItem(GetContent(), null));
                        return item;
                    }
                case Observe:
                    CloseToRootOrDo();
                    Validate();
                    return new PauseItem(null);
                default:
                    throw new StoryboardParsingException(reader, $"O elemento '{reader.LocalName}' não é suportado dentro do elemento 'prompt'.");
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
                if (reader.LocalName == When)
                    condition = ConditionParser.Parse(reader.Value);
                else if (reader.LocalName == Tip)
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
    }
}
