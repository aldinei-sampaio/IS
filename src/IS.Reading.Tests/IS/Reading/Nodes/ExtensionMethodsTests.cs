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
    public void IsMainCharacter(string personName, string mainCharacterName, bool expected)
    {
        var state = A.Dummy<INavigationState>();
        state.PersonName = personName;
        state.MainCharacterName = mainCharacterName;
        state.IsMainCharacter().Should().Be(expected);
    }
}
