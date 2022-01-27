namespace IS.Reading.Variables;

public class TextSourceParserResultTests
{
    [Fact]
    public void ErrorMessage()
    {
        var sut = new TextSourceParserResult("string");
        sut.IsError.Should().BeTrue();
        sut.ErrorMessage.Should().Be("string");
        Assert.Throws<InvalidOperationException>(() => sut.TextSource);
    }

    [Fact]
    public void TextSource()
    {
        var textSource = A.Dummy<ITextSource>();
        var sut = new TextSourceParserResult(textSource);
        sut.IsError.Should().BeFalse();
        sut.TextSource.Should().BeSameAs(textSource);
        Assert.Throws<InvalidOperationException>(() => sut.ErrorMessage);
    }

    [Fact]
    public void ConstructorWithNoArguments()
    {
        var sut = new TextSourceParserResult();
        sut.IsError.Should().BeTrue();
        Assert.Throws<InvalidOperationException>(() => sut.TextSource);
        Assert.Throws<InvalidOperationException>(() => sut.ErrorMessage);
    }
}
