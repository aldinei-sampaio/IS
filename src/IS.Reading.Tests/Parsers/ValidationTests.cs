using FluentAssertions;
using System.IO;
using System.Xml;
using Xunit;

namespace IS.Reading.Parsers
{
    public class ValidationTests
    {
        [Fact]
        public void NoStoryboardElement()
        {
            var data = "<test></test>";
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Load(data));
            ex.Message.Should().Be("Elemento 'storyboard' não encontrado.\r\nLinha 1");
        }

        [Fact]
        public void NotSupportedElementInStoryboard()
        {
            var data = "<storyboard><test /></storyboard>";
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Load(data));
            ex.Message.Should().Be("O elemento 'test' não é suportado.\r\nLinha 1");
        }

        [Fact]
        public void ProtagonistWithWhen()
        {
            var data = "<storyboard><protagonist when=\"a\" /></storyboard>";
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Load(data));
            ex.Message.Should().Be("O elemento 'protagonist' não suporta condições 'when'.\r\nLinha 1");
        }

        [Fact]
        public void InterlocutorWithWhen()
        {
            var data = "<storyboard><person when=\"a\">teste</person></storyboard>";
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Load(data));
            ex.Message.Should().Be("O elemento 'person' não suporta condições 'when'.\r\nLinha 1");
        }

        [Fact]
        public void WhenInsidePrompt()
        {
            var data = "<storyboard><protagonist /><prompt><a>Teste</a><voice when=\"a\">Teste</voice></prompt></storyboard>";
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Load(data));
            ex.Message.Should().Be("Não são permitidos atributos no elemento 'voice' dentro do elemento 'prompt'.\r\nLinha 1");
        }

        [Theory]
        [InlineData("music")]
        [InlineData("background")]
        [InlineData("do")]
        [InlineData("emotion")]
        [InlineData("viewpoint")]
        [InlineData("protagonist")]
        [InlineData("bump")]
        [InlineData("reward")]
        [InlineData("set")]
        [InlineData("trophy")]
        [InlineData("minigame")]
        [InlineData("prompt")]
        [InlineData("test")]
        public void InvalidElementInsidePrompt(string elementName)
        {
            var data = $"<storyboard><protagonist /><prompt><a>Teste</a><{elementName} /></prompt></storyboard>";
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Load(data));
            ex.Message.Should().Be($"O elemento '{elementName}' não é suportado dentro do elemento 'prompt'.\r\nLinha 1");
        }

        [Theory]
        [InlineData("<observe />")]
        [InlineData("<voice>Teste</voice>")]
        [InlineData("<thought>Teste</thought>")]
        [InlineData("<tutorial>Teste</tutorial>")]
        [InlineData("<narration>Teste</narration>")]
        public void ValidElementInsidePrompt(string element)
        {
            var data = $"<storyboard><protagonist /><prompt><a>Teste</a>{element}</prompt></storyboard>";
            StoryboardParser.Load(data);
        }

        [Theory]
        [InlineData("observe")]
        [InlineData("protagonist")]
        [InlineData("bump")]
        public void ElementMustBeEmpty(string elementName)
        {
            var data = $"<storyboard><protagonist /><{elementName}>Teste</{elementName}></storyboard>";
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Load(data));
            ex.Message.Should().Be($"O elemento '{elementName}' não pode ter conteúdo.\r\nLinha 1");
        }

        [Theory]
        [InlineData("music")]
        [InlineData("background")]
        [InlineData("set")]
        [InlineData("unset")]
        [InlineData("tutorial")]
        [InlineData("narration")]
        [InlineData("voice")]
        [InlineData("thought")]
        [InlineData("person")]
        public void ElementMustHaveContent(string elementName)
        {
            var data = $"<storyboard><protagonist /><{elementName} /></storyboard>";
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Load(data));
            ex.Message.Should().Be($"Conteúdo é requerido para o elemento '{elementName}'.\r\nLinha 1");
        }

        [Theory]
        [InlineData("<prompt />")]
        [InlineData("<prompt/>")]
        public void EmptyPromptElement(string element)
        {
            var data = $"<storyboard><protagonist />{element}</storyboard>";
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Load(data));
            ex.Message.Should().Be($"O elemento 'prompt' não pode estar vazio.\r\nLinha 1");
        }

        [Fact]
        public void PromptWithoutOptions()
        {
            var data = $"<storyboard><protagonist /><prompt></prompt></storyboard>";
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Load(data));
            ex.Message.Should().Be($"Nenhuma escolha definida no elemento 'prompt'. Favor definir um ou mais elementos de 'a' a 'z'.\r\nLinha 1");
        }

        [Fact]
        public void PromptWithInvalidTime()
        {
            var data = $"<storyboard><protagonist /><prompt time=\"abc\"></prompt></storyboard>";
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Load(data));
            ex.Message.Should().Be($"O valor 'abc' não é válido para o atributo 'time'. É esperado um número inteiro.\r\nLinha 1");
        }

        [Fact]
        public void PromptWithInvalidDefault()
        {
            var data = $"<storyboard><protagonist /><prompt default=\"1\"></prompt></storyboard>";
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Load(data));
            ex.Message.Should().Be($"O valor '1' não é válido para o atributo 'default'. É esperada uma opção de 'a' a 'z'.\r\nLinha 1");
        }

        [Fact]
        public void PromptWithInvalidRandomOrder()
        {
            var data = $"<storyboard><protagonist /><prompt randomorder=\"abc\"></prompt></storyboard>";
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Load(data));
            ex.Message.Should().Be($"O valor 'abc' não é válido para o atributo 'randomorder'. É esperado '1' ou '0'.\r\nLinha 1");
        }

        [Fact]
        public void PromptWithInvalidAttribute()
        {
            var data = $"<storyboard><protagonist /><prompt teste=\"abc\"></prompt></storyboard>";
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Load(data));
            ex.Message.Should().Be("O atributo 'teste' não é suportado para o elemento 'prompt'.\r\nLinha 1");
        }

        [Fact]
        public void PromptWithInvalidChild()
        {
            var data = $"<storyboard><protagonist /><prompt><aa>teste</aa></prompt></storyboard>";
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Load(data));
            ex.Message.Should().Be("O elemento 'aa' não é suportado dentro do elemento 'prompt'.\r\nLinha 1");
        }

        [Theory]
        [InlineData("<a />")]
        [InlineData("<a></a>")]
        public void EmptyChoice(string element)
        {
            var data = $"<storyboard><protagonist /><prompt>{element}</prompt></storyboard>";
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Load(data));
            ex.Message.Should().Be("O elemento 'a' precisa possuir conteúdo.\r\nLinha 1");
        }

        [Fact]
        public void ChoiceWithInvalidAttribute()
        {
            var data = $"<storyboard><protagonist /><prompt><a teste=\"abc\">teste</a></prompt></storyboard>";
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Load(data));
            ex.Message.Should().Be("O atributo 'teste' não é suportado para o elemento 'a'.\r\nLinha 1");
        }

        [Fact]
        public void ChoiceWithInvalidChild()
        {
            var data = $"<storyboard><protagonist /><prompt><a><b /></a></prompt></storyboard>";
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Load(data));
            ex.Message.Should().Be("O elemento 'b' não é suportado como filho do elemento 'a'.\r\nLinha 1");
        }

        [Theory]
        [InlineData("viewpoint")]
        [InlineData("observe")]
        [InlineData("bump")]
        public void InvalidConditionForEmptyElements(string elementName)
        {
            var data = $"<storyboard><protagonist /><{elementName} when=\"a=2\" /></storyboard>";
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Load(data));
            ex.Message.Should().Be("O valor 'a=2' não é válido para o atributo 'when'.\r\nLinha 1");
        }

        [Theory]
        [InlineData("background")]
        [InlineData("music")]
        [InlineData("set")]
        [InlineData("unset")]
        [InlineData("prompt")]
        [InlineData("voice")]
        [InlineData("thought")]
        [InlineData("narration")]
        [InlineData("tutorial")]
        [InlineData("emotion")]
        public void InvalidConditionForNonEmptyElements(string elementName)
        {
            var data = $"<storyboard><protagonist /><{elementName} when=\"a=2\"></{elementName}></storyboard>";
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Load(data));
            ex.Message.Should().Be("O valor 'a=2' não é válido para o atributo 'when'.\r\nLinha 1");
        }

        [Theory]
        [InlineData("narration")]
        [InlineData("tutorial")]
        [InlineData("voice")]
        [InlineData("thought")]
        public void InvalidConditionForRepeatedElement(string elementName)
        {
            var data = $"<storyboard><protagonist /><{elementName}>Teste1</{elementName}><{elementName} when=\"a..b\">Teste2</{elementName}></storyboard>";
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Load(data));
            ex.Message.Should().Be("O valor 'a..b' não é válido para o atributo 'when'.\r\nLinha 1");
        }
    }
}
