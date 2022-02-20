﻿using IS.Reading.Variables;

namespace IS.Reading.Parsing.NodeParsers.PersonParsers;

public class SpeechChildNodeParserTests
{
    [Fact]
    public void Initialization()
    {
        var elementParser = A.Dummy<IElementParser>();
        var textSourceParser = A.Dummy<ITextSourceParser>();
        var choiceNodeParser = Helper.FakeParser<IChoiceNodeParser>("?");
        var sut = new SpeechChildNodeParser(elementParser, textSourceParser, choiceNodeParser);

        sut.Name.Should().Be("-");
        sut.IsArgumentRequired.Should().BeTrue();
        sut.ElementParser.Should().BeSameAs(elementParser);
        sut.TextSourceParser.Should().BeSameAs(textSourceParser);
        sut.Settings.Should().BeOfType<ElementParserSettings.AggregatedNonRepeat>();
        sut.Settings.ChildParsers.Should().BeEquivalentTo(choiceNodeParser);
    }
}