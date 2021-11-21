using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace IS.Reading.Parsers
{
    public class TrophiesParser
    {
        private const string Trophies = "trophies";
        private const string Title = "title";
        private const string Description = "description";
        private const string Requirement = "requirement";

        private readonly XmlReader reader;
        private readonly Dictionary<string, Trophy> trophies;

        private TrophiesParser(XmlReader reader)
        {
            this.reader = reader;
            trophies = new Dictionary<string, Trophy>();
        }

        public static Dictionary<string, Trophy> Parse(Stream stream)
            => Parse(new StreamReader(stream));

        public static Dictionary<string, Trophy> Parse(string content)
            => Parse(new StringReader(content));

        public static Dictionary<string, Trophy> Parse(TextReader textReader)
        {
            using var reader = XmlReader.Create(textReader);
            var parser = new TrophiesParser(reader);

            reader.MoveToContent();

            if (reader.LocalName != Trophies)
                throw new StoryboardParsingException(reader, $"Elemento '{Trophies}' não encontrado.");

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                    parser.HandleStartElement();
            }

            return parser.trophies;
        }

        private void HandleStartElement()
        {
            var name = reader.LocalName;

            if (trophies.ContainsKey(name))
                throw new StoryboardParsingException(reader, $"Elemento duplicado: '{name}'.");

            string? title = null;
            string? description = null;
            string? requirement = null;

            for(; ; )
            {
                reader.Read();
                    
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.LocalName)
                    {
                        case Title:
                            title = GetContent();
                            break;
                        case Description:
                            description = GetContent();
                            break;
                        case Requirement:
                            requirement = GetContent();
                            break;
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (reader.LocalName == name)
                        break;

                    switch (reader.LocalName)
                    {
                        case Title:
                        case Description:
                        case Requirement:
                            break;
                        default:
                            throw new StoryboardParsingException(reader, $"Fim de elemento não esperado: '{reader.LocalName}'.");
                    }
                }
            }

            if (title == null)
                throw new StoryboardParsingException(reader, $"Elemento '{Title}' não encontrado.");

            if (description == null)
                throw new StoryboardParsingException(reader, $"Elemento '{Description}' não encontrado.");

            if (requirement == null)
                throw new StoryboardParsingException(reader, $"Elemento '{Requirement}' não encontrado.");

            trophies.Add(name, new Trophy(name, title, description, requirement));
        }

        private string GetContent()
        {
            var elementName = reader.LocalName;
            reader.Read();
            if (reader.NodeType != XmlNodeType.Text)
                throw new StoryboardParsingException(reader, $"Conteúdo é requerido para o elemento '{elementName}'.");
            var value = reader.ReadContentAsString();
            if (string.IsNullOrEmpty(value))
                throw new StoryboardParsingException(reader, $"Conteúdo é requerido para o elemento '{elementName}'.");
            return value;
        }
    }
}
