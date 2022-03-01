namespace IS.Reading.Variables;

public class VariableTextSourceTests
{
    [Theory]
    [InlineData("alpha", "Teste1")]
    [InlineData("beta", "")]
    public void SimpleTest(string varName, string buildResult)
    {
        var variables = A.Fake<IVariableDictionary>(i => i.Strict());
        A.CallTo(() => variables["alpha"]).Returns("Teste1");
        A.CallTo(() => variables["beta"]).Returns(null);

        var sut = new VariableTextSource(varName);
        sut.VariableName.Should().Be(varName);
        sut.ToString().Should().Be($"{{{varName}}}");
        sut.Build(variables).Should().Be(buildResult);
    }

}