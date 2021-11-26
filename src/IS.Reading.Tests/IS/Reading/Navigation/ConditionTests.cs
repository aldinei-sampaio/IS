using IS.Reading.Parsing;
using IS.Reading.State;

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

    [Theory]
    [InlineData("a", ConditionType.LessThan, 2, 0, "!a[2:]")]
    [InlineData("b,c", ConditionType.LessThan, -9, 599, "!b,c[-9:]")]
    [InlineData("c", ConditionType.EqualOrLessThan, 1, 0, "c[:1]")]
    [InlineData("d,e,f", ConditionType.EqualOrLessThan, -7, 831, "d,e,f[:-7]")]
    [InlineData("e", ConditionType.GreaterThan, 3, 0, "!e[:3]")]
    [InlineData("f,g", ConditionType.GreaterThan, -8, 324, "!f,g[:-8]")]
    [InlineData("g", ConditionType.EqualOrGreaterThan, 6, 0, "g[6:]")]
    [InlineData("h,i", ConditionType.EqualOrGreaterThan, -5, 28, "h,i[-5:]")]
    [InlineData("i", ConditionType.Between, 1, 5, "i[1:5]")]
    [InlineData("j,k,l", ConditionType.Between, -2, -1, "j,k,l[-2:-1]")]
    [InlineData("k", ConditionType.Between, -1, 3, "k[-1:3]")]
    [InlineData("l,m,n,o", ConditionType.NotBetween, 4, 6, "!l,m,n,o[4:6]")]
    [InlineData("m", ConditionType.NotBetween, -8, -5, "!m[-8:-5]")]
    [InlineData("n,o", ConditionType.NotBetween, -3, 4, "!n,o[-3:4]")]
    [InlineData("o", ConditionType.Defined, 3, 8, "o[1:]")]
    [InlineData("p,q,r", ConditionType.Defined, -9, -1, "p,q,r[1:]")]
    [InlineData("q", ConditionType.Undefined, 5, 5, "q[:0]")]
    [InlineData("r,s", ConditionType.Undefined, -3, -2, "r,s[:0]")]
    [InlineData("s", ConditionType.EqualTo, 5, 0, "s[5]")]
    [InlineData("t,u,v", ConditionType.EqualTo, -7, 951, "t,u,v[-7]")]
    [InlineData("u", ConditionType.NotEqualTo, 6, 0, "!u[6]")]
    [InlineData("v,w,x,y,z", ConditionType.NotEqualTo, -1, 357, "!v,w,x,y,z[-1]")]
    public void ToStringTest(string varName, ConditionType type, int min, int max, string expected)
    {
        var sut = new Condition(varName.Split(','), type, min, max);
        sut.ToString().Should().Be(expected);
    }
}
