namespace IS.Reading.Variables;

public class ReverseIntegerIncrementTests
{
    public class TestData : TheoryData<string, int, object>
    {
        public TestData()
        {
            Add("alpha", 0, null);
            Add("beta", 1, 0);
            Add("gamma", -58, "abc");
            Add("delta", int.MaxValue, int.MinValue);
            Add("epsilon", int.MinValue, int.MaxValue);
        }
    }

    [Theory]
    [ClassData(typeof(TestData))]
    public void Initialization(string name, int increment, object oldValue)
    {
        var sut = new ReversedIntegerIncrement(name, increment, oldValue);
        sut.Should().BeEquivalentTo(new
        {
            Name = name,
            Increment = increment,
            OldValue = oldValue
        });
    }

    [Theory]
    [ClassData(typeof(TestData))]
    public void ExecuteShouldReturnIntegerIncrement(string name, int increment, object oldValue)
    {
        var variables = A.Dummy<IVariableDictionary>();

        var sut = new ReversedIntegerIncrement(name, increment, oldValue);
        var result = sut.Execute(variables);
        result.Should().BeOfType<IntegerIncrement>()
            .Which.Should().BeEquivalentTo(new
            {
                Name = name,
                Increment = increment
            });
    }

    [Theory]
    [ClassData(typeof(TestData))]
    public void ExecuteMustSetOldValue(string name, int increment, object oldValue)
    {
        var variables = A.Fake<IVariableDictionary>(i => i.Strict());
        A.CallToSet(() => variables[name]).To(oldValue).DoesNothing();

        var sut = new ReversedIntegerIncrement(name, increment, oldValue);
        sut.Execute(variables);

        A.CallToSet(() => variables[name]).To(oldValue).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [InlineData("alpha", null, "alpha=Null")]
    [InlineData("beta", 0, "beta=0")]
    [InlineData("gamma", 5, "gamma=5")]
    [InlineData("delta", 12, "delta=12")]
    [InlineData("epsilon", -131, "epsilon=-131")]
    [InlineData("pi", "", "pi=''")]
    [InlineData("rho", "abc", "rho='abc'")]
    public void ToStringTest(string name, object value, string expected)
    {
        var sut = new ReversedIntegerIncrement(name, 0, value);
        sut.ToString().Should().Be(expected);
    }
}
