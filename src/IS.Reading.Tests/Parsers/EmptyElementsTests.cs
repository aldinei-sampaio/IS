using FluentAssertions;
using Xunit;

namespace IS.Reading.Parsers
{
    public class EmptyElementsTests
    {
        [Theory]
        [InlineData("observe")]
        [InlineData("protagonist")]
        public void RootElements(string elementName)
        {
            var data = $"<storyboard><{elementName}>abcde</{elementName}></storyboard>";
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Parse(data));
            ex.Message.Should().Be($"O elemento '{elementName}' não pode ter conteúdo.\r\nLinha 1");
        }

        [Theory]
        [InlineData("protagonist", "bump")]
        public void InsideElements(string parentNode, string nodeName)
        {
            var data = $"<storyboard><{parentNode} /><{nodeName}>abcde</{nodeName}></storyboard>";
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Parse(data));
            ex.Message.Should().Be($"O elemento '{nodeName}' não pode ter conteúdo.\r\nLinha 1");
        }

        [Fact]
        public void NoWhen()
        {
            var data = "<storyboard><protagonist when=\"a\" /></storyboard>";
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Parse(data));
            ex.Message.Should().Be("O elemento 'protagonist' não suporta condições 'when'.\r\nLinha 1");
        }

        [Theory]
        [InlineData("observe")]
        [InlineData("protagonist")]
        public void NoRandomAttribute(string elementName)
        {
            var data = $"<storyboard><{elementName} teste=\"abc\" /></storyboard>";
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Parse(data));
            ex.Message.Should().Be($"O atributo 'teste' não é suportado para o elemento '{elementName}'.\r\nLinha 1");
        }

    }
}
