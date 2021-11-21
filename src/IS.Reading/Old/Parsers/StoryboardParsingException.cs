using System;
using System.Xml;

namespace IS.Reading.Parsers
{
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
