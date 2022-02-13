using System.Text.RegularExpressions;

namespace IS.Reading.Parsing.ArgumentParsers;

public class ImageArgumentParser : IImageArgumentParser
{
    public Result<string> Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Fail<string>("Era esperado um argumento com o nome da imagem.");

        if (Regex.IsMatch(value, @"[^A-Za-z0-9_\r\n\. ]"))
            return Result.Fail<string>($"O texto '{value}' contém caracteres inválidos.");

        var result = value.ReplaceLineEndings(string.Empty).Trim();

        if (result.Length > 64)
            return Result.Fail<string>($"O texto contém {result.Length} caracteres, o que excede a quantidade máxima de 64.");
            
        return Result.Ok(result);
    }
}
