namespace IS.Reading.Parsing;

public class ElementNotRecognizedException : ParsingException
{
    public ElementNotRecognizedException(string elementName)
        : base($"Elemento não reconhecido: {elementName}")
    {
    }
}
