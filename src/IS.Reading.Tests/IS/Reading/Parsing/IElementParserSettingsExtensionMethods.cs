using IS.Reading.Navigation;

namespace IS.Reading.Parsing;

public static class IBlockExtensionMethods
{
    public static void ShouldContainOnly(this IBlock block, INode node)
    {
        block.ForwardQueue.Count.Should().Be(1);
        block.ForwardQueue.Peek().Should().BeSameAs(node);
    }
}

public static class IElementParserSettingsExtensionMethods
{
    public static void ShouldContainOnly(this IElementParserSettings settings, params object[] parsers)
    {
        var attributeParserCount = 0;
        var nodeParserCount = 0;
        var textParserFound = false;
        foreach(var parser in parsers)
        {
            if (parser is ITextParser)
            {
                settings.TextParser.Should().BeSameAs(parser);
                textParserFound = true;
            }
            else if (parser is IAttributeParser attributeParser)
            {
                settings.AttributeParsers[attributeParser.Name].Should().BeSameAs(attributeParser);
                attributeParserCount++;
            }
            else if (parser is INodeParser nodeParser)
            {
                settings.ChildParsers[nodeParser.Name].Should().BeSameAs(nodeParser);
                nodeParserCount++;
            }
            else
            {
                throw new InvalidOperationException("Objeto não é um parser.");
            }
        }
        settings.AttributeParsers.Count.Should().Be(0);
        settings.ChildParsers.Count.Should().Be(0);
        if (!textParserFound)
            settings.TextParser.Should().BeNull();
    }

    public static void ShouldBeNoRepeat(this IElementParserSettings settings, params object[] parsers)
    {
        settings.ShouldContainOnly(parsers);
        settings.NoRepeatNode.Should().BeTrue();
        settings.ExitOnUnknownNode.Should().BeTrue();
    }

    public static void ShouldBeAggregate(this IElementParserSettings settings, params object[] parsers)
    {
        settings.ShouldContainOnly(parsers);
        settings.NoRepeatNode.Should().BeFalse();
        settings.ExitOnUnknownNode.Should().BeTrue();
    }

    public static void ShouldBeNormal(this IElementParserSettings settings, params object[] parsers)
    {
        settings.ShouldContainOnly(parsers);
        settings.NoRepeatNode.Should().BeFalse();
        settings.ExitOnUnknownNode.Should().BeFalse();
    }
}
