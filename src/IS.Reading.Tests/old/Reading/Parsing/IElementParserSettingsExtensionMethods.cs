﻿using IS.Reading.Navigation;

namespace IS.Reading.Parsing;

public static class IBlockExtensionMethods
{
    public static void ShouldContain(this IBlock block, params Action<INode>[] validations)
    {
        block.Nodes.Count.Should().Be(validations.Length);
        for (var n = 0; n < validations.Length; n++)
            block.Nodes[n].ShouldSatisfy(validations[n]);
    }

    public static void ShouldBeEquivalentTo(this IBlock block, params INode[] nodes)
        => block.Nodes.Should().BeEquivalentTo(nodes);

    public static void ShouldContainOnly(this IBlock block, INode node)
        => block.Nodes.Should().ContainSingle().Which.Should().BeSameAs(node);

    public static void ShouldBeEmpty(this IBlock block)
        => block.Nodes.Should().BeEmpty();
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
        settings.AttributeParsers.Count.Should().Be(attributeParserCount);
        settings.ChildParsers.Count.Should().Be(nodeParserCount);
        if (!textParserFound)
            settings.TextParser.Should().BeNull();
    }

    public static void ShouldBeNonRepeat(this IElementParserSettings settings, params object[] parsers)
    {
        settings.ShouldContainOnly(parsers);
        settings.NoRepeatNode.Should().BeTrue();
        settings.ExitOnUnknownNode.Should().BeFalse();
    }

    public static void ShouldBeAggregatedNonRepeat(this IElementParserSettings settings, params object[] parsers)
    {
        settings.ShouldContainOnly(parsers);
        settings.NoRepeatNode.Should().BeTrue();
        settings.ExitOnUnknownNode.Should().BeTrue();
    }

    public static void ShouldBeAggregated(this IElementParserSettings settings, params object[] parsers)
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