using IS.Reading.Variables;

namespace IS.Reading.Nodes;

public class InputBuilderTests
{
    [Fact]
    public void Initialization()
    {
        var key = "alpha";
        var sut = new InputBuilder(key);
        sut.Key.Should().Be(key);
        sut.TitleSource.Should().BeNull();
        sut.TextSource.Should().BeNull();
        sut.MaxLength.Should().Be(InputBuilder.MaxLenghtDefaultValue);
        sut.ConfirmationSource.Should().BeNull();
    }

    [Fact]
    public void ReadWriteProperties()
    {
        var key = "alpha";
        var titleSource = A.Dummy<ITextSource>();
        var textSource = A.Dummy<ITextSource>();
        var maxLength = InputBuilder.MaxLenghtDefaultValue - 1;
        var confirmationSource = A.Dummy<ITextSource>();

        var sut = new InputBuilder(key)
        {
            TitleSource = titleSource,
            TextSource = textSource,
            MaxLength = maxLength,
            ConfirmationSource = confirmationSource
        };

        sut.Key.Should().Be(key);
        sut.TitleSource.Should().BeSameAs(titleSource);
        sut.TextSource.Should().BeSameAs(textSource);
        sut.MaxLength.Should().Be(maxLength);
        sut.ConfirmationSource.Should().BeSameAs(confirmationSource);
    }

    [Fact]
    public void BuildEventWithDefaults()
    {
        var key = "beta";
        var variableDictionary = A.Fake<IVariableDictionary>(i => i.Strict());
        A.CallTo(() => variableDictionary[key]).Returns(null);

        var sut = new InputBuilder(key);
        var @event = sut.BuildEvent(variableDictionary);

        @event.Should().BeEquivalentTo(new
        {
            Key = key,
            Title = (string)null,
            Text = (string)null,
            MaxLength = InputBuilder.MaxLenghtDefaultValue,
            Confirmation = (string)null,
            DefaultValue = (string)null
        });
    }

    [Fact]
    public void BuildEventWithCustomValues()
    {
        var key = "epsilon";

        var defaultValue = "Default";
        var variableDictionary = A.Fake<IVariableDictionary>(i => i.Strict());
        A.CallTo(() => variableDictionary[key]).Returns(defaultValue);

        var title = "Título";
        var titleSource = A.Fake<ITextSource>(i => i.Strict());
        A.CallTo(() => titleSource.Build(variableDictionary)).Returns(title);

        var text = "Texto";
        var textSource = A.Fake<ITextSource>(i => i.Strict());
        A.CallTo(() => textSource.Build(variableDictionary)).Returns(text);

        var maxLength = InputBuilder.MaxLenghtDefaultValue - 1;

        var confirmation = "Confirmação";
        var confirmationSource = A.Fake<ITextSource>(i => i.Strict());
        A.CallTo(() => confirmationSource.Build(variableDictionary)).Returns(confirmation);

        var sut = new InputBuilder(key)
        {
            TitleSource = titleSource,
            TextSource = textSource,
            MaxLength = maxLength,
            ConfirmationSource = confirmationSource
        };

        var @event = sut.BuildEvent(variableDictionary);

        @event.Should().BeEquivalentTo(new
        {
            Key = key,
            Title = title,
            Text = text,
            MaxLength = maxLength,
            Confirmation = confirmation,
            DefaultValue = defaultValue
        });
    }
}
