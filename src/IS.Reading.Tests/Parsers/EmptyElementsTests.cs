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
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Load(data));
            ex.Message.Should().Be($"O element '{elementName}' não pode ter conteúdo.\r\nLinha 1");
        }

        [Theory]
        [InlineData("protagonist", "bump")]
        public void InsideElements(string parentNode, string nodeName)
        {
            var data = $"<storyboard><{parentNode}><{nodeName}>abcde</{nodeName}</{parentNode}></storyboard>";
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Load(data));
            ex.Message.Should().Be($"O element '{nodeName}' não pode ter conteúdo.\r\nLinha 1");
        }
    }
}
