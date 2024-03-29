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
