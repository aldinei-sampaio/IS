using FluentAssertions;
using Xunit;

namespace IS.Reading.Parsers
{
    public class ObserveTests
    {
        [Fact]
        public void ShouldBeEmpty()
        {
            var data = "<storyboard><observe>abcde</observe></storyboard>";
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Load(data));
            ex.Message.Should().Be("O element 'observe' não pode ter conteúdo.\r\nLinha 1");
        }

    }
}
