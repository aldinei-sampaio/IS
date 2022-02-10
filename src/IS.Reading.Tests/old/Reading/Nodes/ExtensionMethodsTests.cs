using IS.Reading.State;

namespace IS.Reading.Nodes;

public class ExtensionMethodsTests
{
    [Theory]
    [InlineData(null, null, false)]
    [InlineData(null, "alpha", false)]
    [InlineData("alpha", null, false)]
    [InlineData("alpha", "beta", false)]
    [InlineData("alpha", "alpha", true)]
    public void IsProtagonist(string personName, string protagonistName, bool expected)
    {
        var state = A.Dummy<INavigationState>();
        state.PersonName = personName;
        state.ProtagonistName = protagonistName;
        state.IsProtagonist().Should().Be(expected);
    }
}
