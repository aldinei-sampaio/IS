namespace IS.Reading.Variables;

public class VarSetTests
{
    public class TestData : TheoryData<string, object>
    {
        public TestData()
        {
            Add("alpha", null);
            Add("beta", 0);
            Add("gama", 1);
            Add("delta", -158);
            Add("epsilon", int.MaxValue);
            Add("pi", int.MinValue);
            Add("rho", "abc");
            Add("omega", "");
        }
    }

    [Theory]
    [ClassData(typeof(TestData))]
    public void Initialization(string name, object value)
    {
        var sut = new VarSet(name, value);
        sut.Should().BeEquivalentTo(new
        {
            Name = name,
            Value = value
        });
    }

    [Theory]
    [ClassData(typeof(TestData))]
    public void ExecuteShouldSetVariable(string name, object value)
    {
        var variables = A.Fake<IVariableDictionary>(i => i.Strict());
        A.CallTo(() => variables[name]).Returns(null);
        A.CallToSet(() => variables[name]).To(value).DoesNothing();

        var sut = new VarSet(name, value);
        sut.Execute(variables);

        A.CallToSet(() => variables[name]).To(value).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [InlineData("alpha", null, null)]
    [InlineData("beta", 0, null)]
    [InlineData("gamma", 0, 5)]
    [InlineData("delta", 12, int.MinValue)]
    [InlineData("epsilon", -131, "abc")]
    [InlineData("pi", "abc", "def")]
    [InlineData("rho", "abc", int.MaxValue)]
    public void ExecuteShouldReturnOldValue(string name, object oldValue, object value)
    {
        var variables = A.Dummy<IVariableDictionary>();
        A.CallTo(() => variables[name]).Returns(oldValue);

        var sut = new VarSet(name, value);
        var result = sut.Execute(variables);

        result.Should().Be(oldValue);
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
        var sut = new VarSet(name, value);
        sut.ToString().Should().Be(expected);
    }
}
