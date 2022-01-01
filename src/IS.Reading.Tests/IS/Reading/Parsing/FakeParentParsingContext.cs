﻿using IS.Reading.Conditions;
using IS.Reading.Navigation;

namespace IS.Reading.Parsing;

public class FakeParentParsingContext : IParentParsingContext
{
    public List<INode> Nodes { get; } = new();
    public void AddNode(INode node) => Nodes.Add(node);
    public string ParsedText { get; set; } = null;
    public ICondition When { get; set; } = null;
    public ICondition While { get; set; } = null;

    public void ShouldBeEmpty()
    {
        When.Should().BeNull();
        While.Should().BeNull();
        Nodes.Should().BeEmpty();
        ParsedText.Should().BeNull();
    }

    public void ShouldContainSingle<T>(Action<T> validator) where T : INode
    { 
        Nodes.Should().ContainSingle()
            .Which.Should().BeOfType<T>()
            .Which.ShouldSatisfy(validator);
    }

    public void ShouldContainSingle(INode obj)
        => Nodes.Should().ContainSingle().Which.Should().BeSameAs(obj);
}
