using IS.Reading.Parsing;

namespace IS.Reading.Navigation;

public class ConditionTests
{
    [Theory]
    [InlineData("a", 1, true)]
    [InlineData("a", 2, true)]
    [InlineData("a", 1589, true)]
    [InlineData("a", 0, false)]
    [InlineData("a", -1, false)]
    [InlineData("a", -58927, false)]
    [InlineData("!a", 1, false)]
    [InlineData("!a", 0, true)]
    [InlineData("!a", -1, true)]
    [InlineData("a[2]", 1, false)]
    [InlineData("a[2]", 2, true)]
    [InlineData("a[2]", -2, false)]
    [InlineData("!a[1]", 1, false)]
    [InlineData("!a[1]", 57, true)]
    [InlineData("!a[1]", 0, true)]
    [InlineData("!a[1]", -9876, true)]
    [InlineData("a[3:]", 3, true)]
    [InlineData("a[3:]", 4, true)]
    [InlineData("a[3:]", 999999, true)]
    [InlineData("a[3:]", 2, false)]
    [InlineData("a[3:]", 0, false)]
    [InlineData("a[3:]", -6427, false)]
    [InlineData("!a[0:]", -3, true)]
    [InlineData("!a[0:]", -1, true)]
    [InlineData("!a[0:]", 0, false)]
    [InlineData("!a[0:]", 1, false)]
    [InlineData("a[:-1]", 3, false)]
    [InlineData("a[:-1]", 0, false)]
    [InlineData("a[:-1]", -1, true)]
    [InlineData("a[:-1]", -2, true)]
    [InlineData("!a[:-2]", -1, true)]
    [InlineData("!a[:-2]", 0, true)]
    [InlineData("!a[:-2]", 894, true)]
    [InlineData("!a[:-2]", -2, false)]
    [InlineData("!a[:-2]", -487, false)]
    [InlineData("a[-1:2]", 0, true)]
    [InlineData("a[-1:2]", 2, true)]
    [InlineData("a[-1:2]", -1, true)]
    [InlineData("a[-1:2]", 1, true)]
    [InlineData("a[-1:2]", -2, false)]
    [InlineData("a[-1:2]", 3, false)]
    [InlineData("!a[4:7]", 3, true)]
    [InlineData("!a[4:7]", 8, true)]
    [InlineData("!a[4:7]", 0, true)]
    [InlineData("!a[4:7]", -64572, true)]
    [InlineData("!a[4:7]", 852144, true)]
    [InlineData("!a[4:7]", 4, false)]
    [InlineData("!a[4:7]", 5, false)]
    [InlineData("!a[4:7]", 6, false)]
    [InlineData("!a[4:7]", 7, false)]
    [InlineData("a,b,c", 7, true)]
    [InlineData("a,b,c", 0, false)]
    [InlineData("!a,b,c", 7, false)]
    [InlineData("!a,b,c", 0, true)]
    public void Evaluate(string when, int actualValue, bool expectedResult)
    {
        var context = A.Fake<IVariableDictionary>(i => i.Strict());
        A.CallTo(() => context.Get("a")).Returns(actualValue);
        A.CallTo(() => context.Get("b")).Returns(0);
        A.CallTo(() => context.Get("c")).Returns(0);

        var parser = new ConditionParser();
        var condition = parser.Parse(when);
        condition.Should().NotBeNull();
        condition.Evaluate(context).Should().Be(expectedResult);
    }
}
