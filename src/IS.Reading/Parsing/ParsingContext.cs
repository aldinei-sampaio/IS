using System.Text;
using System.Xml;

namespace IS.Reading.Parsing
{
    internal class ParsingContext : IParsingContext
    {
        private const int MaxErrorCount = 10;

        private readonly StringBuilder stringBuilder = new();

        private int errorCount = 0;
        
        public bool IsSuccess => errorCount == 0;

        public void LogError(XmlReader xmlReader, string message)
        {
            errorCount++;
            if (errorCount > MaxErrorCount)
                return;

            if (stringBuilder.Length > 0)
                stringBuilder.AppendLine();

            if (xmlReader is IXmlLineInfo info)
            {
                stringBuilder.Append($"Linha ");
                stringBuilder.Append(info.LineNumber);
                stringBuilder.Append(": ");
            }

            stringBuilder.Append(message);

            if (errorCount == MaxErrorCount)
            {
                stringBuilder.AppendLine();
                stringBuilder.Append("Número máximo de erros atingido.");
            }
        }

        public override string ToString()
            => stringBuilder.ToString();
    }
}
