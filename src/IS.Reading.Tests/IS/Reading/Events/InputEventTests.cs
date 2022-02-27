namespace IS.Reading.Events;



public class InputEventTests
{
    [Theory]
    [InlineData("abc", "Teste", "Entre com algo:", 16, "Confirma?", "Sulana")]
    [InlineData("def", "Título", null, 32, "Tem certeza?", "")]
    [InlineData("xyz", null, "Senha:", 24, "Usar a senha {0}?", null)]
    [InlineData("ghi", "Informação", "Entre seu nome:", 12, null, "Oberon")]
    [InlineData("jkl", null, null, 10, null, null)]
    public void Initialization(string key, string title, string text, int maxLength, string confirmation, string defaultValue)
    {
        var sut = new InputEvent(key, title, text, maxLength, confirmation, defaultValue);
        sut.Key.Should().Be(key);
        sut.Text.Should().Be(text);
        sut.MaxLength.Should().Be(maxLength);
        sut.Confirmation.Should().Be(confirmation);
        sut.DefaultValue.Should().Be(defaultValue);
        sut.ToString().Should().Be($"input: {key}");
    }
}