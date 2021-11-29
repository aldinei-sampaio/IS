using IS.Reading.Navigation;
using System.Text;
using System.Xml;

namespace IS.Reading.Parsing;

public class ParsingContext : IParsingContext
{
    private const int MaxErrorCount = 10;
    private readonly StringBuilder stringBuilder = new();
    private int errorCount = 0;

    public bool IsSuccess => errorCount == 0;

    public List<INode> DismissNodes { get; } = new();

    public string? Person { get; set; }

    public void LogError(XmlReader xmlReader, string message)
    {
        if (errorCount >= MaxErrorCount)
            return;

        errorCount++;

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
