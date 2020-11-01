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
        public void WhenAfterChoice()
        {
            var data = "<storyboard><protagonist /><choice><a>Teste</a></choice><voice when=\"a\">Teste</voice></storyboard>";
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Load(data));
            ex.Message.Should().Be("Não é permitido incluir condição 'when' no elemento seguinte a 'choice'.\r\nLinha 1");
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
        [InlineData("choice")]
        [InlineData("test")]
        public void InvalidElementAfterChoice(string elementName)
        {
            var data = $"<storyboard><protagonist /><choice><a>Teste</a></choice><{elementName} /></storyboard>";
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Load(data));
            ex.Message.Should().Be($"O elemento '{elementName}' não pode vir depois de um elemento 'choice'.\r\nLinha 1");
        }

        [Theory]
        [InlineData("<observe />")]
        [InlineData("<voice>Teste</voice>")]
        [InlineData("<thought>Teste</thought>")]
        [InlineData("<tutorial>Teste</tutorial>")]
        [InlineData("<narration>Teste</narration>")]
        public void ValidElementAfterChoice(string element)
        {
            var data = $"<storyboard><protagonist /><choice><a>Teste</a></choice>{element}</storyboard>";
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
        public void ElementMustHaveContent(string elementName)
        {
            var data = $"<storyboard><protagonist /><{elementName} /></storyboard>";
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Load(data));
            ex.Message.Should().Be($"Conteúdo é requerido para o elemento '{elementName}'.\r\nLinha 1");
        }

        [Theory]
        [InlineData("<choice />")]
        [InlineData("<choice/>")]
        public void EmptyChoiceElement(string element)
        {
            var data = $"<storyboard><protagonist />{element}</storyboard>";
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Load(data));
            ex.Message.Should().Be($"O elemento 'choice' não pode estar vazio.\r\nLinha 1");
        }

        [Fact]
        public void ChoiceWithoutOptions()
        {
            var data = $"<storyboard><protagonist /><choice></choice></storyboard>";
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Load(data));
            ex.Message.Should().Be($"Nenhuma opção definida no elemento 'choice'. Favor definir um ou mais elementos de 'a' a 'z'.\r\nLinha 1");
        }

        [Fact]
        public void ChoiceWithInvalidTime()
        {
            var data = $"<storyboard><protagonist /><choice time=\"abc\"></choice></storyboard>";
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Load(data));
            ex.Message.Should().Be($"O valor 'abc' não é válido para o atributo 'time'. É esperado um número inteiro.\r\nLinha 1");
        }

        [Fact]
        public void ChoiceWithInvalidTimeout()
        {
            var data = $"<storyboard><protagonist /><choice timeout=\"1\"></choice></storyboard>";
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Load(data));
            ex.Message.Should().Be($"O valor '1' não é válido para o atributo 'timeout'. É esperada uma opção de 'a' a 'z'.\r\nLinha 1");
        }

        [Fact]
        public void ChoiceWithInvalidRandomOrder()
        {
            var data = $"<storyboard><protagonist /><choice randomorder=\"abc\"></choice></storyboard>";
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Load(data));
            ex.Message.Should().Be($"O valor 'abc' não é válido para o atributo 'randomorder'. É esperado '1' ou '0'.\r\nLinha 1");
        }

        [Fact]
        public void ChoiceWithInvalidAttribute()
        {
            var data = $"<storyboard><protagonist /><choice teste=\"abc\"></choice></storyboard>";
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Load(data));
            ex.Message.Should().Be("O atributo 'teste' não é suportado para o elemento 'choice'.\r\nLinha 1");
        }

        [Fact]
        public void ChoiceWithInvalidChild()
        {
            var data = $"<storyboard><protagonist /><choice><aa>teste</aa></choice></storyboard>";
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Load(data));
            ex.Message.Should().Be("O elemento 'aa' não é suportado dentro do elemento 'choice'.\r\nLinha 1");
        }

        [Theory]
        [InlineData("<a />")]
        [InlineData("<a></a>")]
        public void EmptyChoiceOption(string element)
        {
            var data = $"<storyboard><protagonist /><choice>{element}</choice></storyboard>";
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Load(data));
            ex.Message.Should().Be("O elemento 'a' precisa possuir conteúdo.\r\nLinha 1");
        }

        [Fact]
        public void ChoiceOptionWithInvalidAttribute()
        {
            var data = $"<storyboard><protagonist /><choice><a teste=\"abc\">teste</a></choice></storyboard>";
            var ex = Assert.Throws<StoryboardParsingException>(() => StoryboardParser.Load(data));
            ex.Message.Should().Be("O atributo 'teste' não é suportado para o elemento 'a'.\r\nLinha 1");
        }

        [Fact]
        public void ChoiceOptionWithInvalidChild()
        {
            var data = $"<storyboard><protagonist /><choice><a><b /></a></choice></storyboard>";
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
        [InlineData("choice")]
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
