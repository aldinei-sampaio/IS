using IS.Reading.Navigation;

namespace IS.Reading.Nodes;

public class DismissNodeTests
{
    [Fact]
    public void ConstructorShouldInitializeChangeNodeProperty()
    {
        var node = A.Fake<INode>(i => i.Strict());
        var sut = new DismissNode<INode>(node);
        sut.ChangeNode.Should().BeSameAs(node);
    }

    [Fact]
    public async Task EnterAsyncShouldBeDelegatedToOriginalNode()
    {
        var context = A.Dummy<INavigationContext>();
        var node = A.Fake<INode>(i => i.Strict());
        var reversedNode = A.Fake<INode>(i => i.Strict());
        A.CallTo(() => node.ToString()).Returns("[node]");
        A.CallTo(() => node.EnterAsync(context)).Returns(reversedNode);
        A.CallTo(() => reversedNode.ToString()).Returns("[reversedNode]");
        A.CallTo(() => reversedNode.EnterAsync(context)).Returns(node);

        var sut = new DismissNode<INode>(node);

        sut = (DismissNode<INode>)await sut.EnterAsync(context);
        sut.ChangeNode.Should().BeSameAs(reversedNode);

        sut = (DismissNode<INode>)await sut.EnterAsync(context);
        sut.ChangeNode.Should().BeSameAs(node);

        A.CallTo(() => node.EnterAsync(context)).MustHaveHappenedOnceExactly();
        A.CallTo(() => node.EnterAsync(context)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ShouldReturnItselfWhenSameNodeIsReturned()
    {
        var context = A.Dummy<INavigationContext>();
        var node = A.Fake<INode>(i => i.Strict(StrictFakeOptions.AllowObjectMethods));
        A.CallTo(() => node.EnterAsync(context)).Returns(node);

        var sut = new DismissNode<INode>(node);

        var result = (DismissNode<INode>)await sut.EnterAsync(context);
        result.Should().BeSameAs(sut);
    }
}
