using FluentAssertions;
using Xunit;

namespace IS.Reading.Parsers
{
    public class VariableNameElementsTests
    {
        [Theory]
        [InlineData("background")]
        [InlineData("music")]
        [InlineData("viewpoint")]
        [InlineData("unset")]
        [InlineData("set")]
        public void EmptyTag(string elementName)
        {
            var data = $"<storyboard><{elementName} /></storyboard>";
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Parse(data));
            ex.Message.Should().Be($"Conteúdo é requerido para o elemento '{elementName}'.\r\nLinha 1");
        }

        [Theory]
        [InlineData("background", "++teste")]
        [InlineData("background", "ar da graça")]
        [InlineData("background", "acentuação")]
        [InlineData("background", "MAIUSCULAS")]
        [InlineData("background", "guarda-chuva")]
        [InlineData("music", "++teste")]
        [InlineData("music", "ar da graça")]
        [InlineData("music", "acentuação")]
        [InlineData("music", "MAIUSCULAS")]
        [InlineData("music", "guarda-chuva")]
        [InlineData("viewpoint", "++teste")]
        [InlineData("viewpoint", "ar da graça")]
        [InlineData("viewpoint", "acentuação")]
        [InlineData("viewpoint", "MAIUSCULAS")]
        [InlineData("viewpoint", "guarda-chuva")]
        [InlineData("unset", "++teste")]
        [InlineData("unset", "ar da graça")]
        [InlineData("unset", "acentuação")]
        [InlineData("unset", "MAIUSCULAS")]
        [InlineData("unset", "guarda-chuva")]
        [InlineData("set", "a+b+c")]
        [InlineData("set", "ar da graça")]
        [InlineData("set", "acentuação")]
        [InlineData("set", "MAIUSCULAS")]
        [InlineData("set", "guarda-chuva")]
        public void InvalidValue(string elementName, string value)
        {
            var data = $"<storyboard><{elementName}>{value}</{elementName}></storyboard>";
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Parse(data));
            ex.Message.Should().Be($"O valor '{value}' não é válido para o elemento '{elementName}'.\r\nLinha 1");
        }
    }
}
