using IS.Reading.Navigation;

namespace IS.Reading.Parsing;

public class FakeParentParsingContext : IParentParsingContext
{
    public List<INode> Nodes { get; } = new();
    public void AddNode(INode node) => Nodes.Add(node);
    public void ShouldBeEmpty() => Nodes.Should().BeEmpty();

    public void ShouldContainSingle<T>(Action<T> validator) where T : INode
    { 
        Nodes.Should().ContainSingle()
            .Which.Should().BeOfType<T>()
            .Which.ShouldSatisfy(validator);
    }

    public void ShouldContainSingle<T>() where T : INode
    {
        Nodes.Should().ContainSingle()
            .Which.Should().BeOfType<T>();
    }

    public void ShouldContainSingle(INode obj)
        => Nodes.Should().ContainSingle().Which.Should().BeSameAs(obj);

    public void ShouldContain(params Action<INode>[] validators)
    {
        Nodes.Count().Should().Be(validators.Length);
        for(var n = 0; n < validators.Length; n++)
            Nodes[n].ShouldSatisfy(validators[n], $"Nodes[{n}]");
    }
}
