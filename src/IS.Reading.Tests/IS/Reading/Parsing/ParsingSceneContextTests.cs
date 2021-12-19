namespace IS.Reading.Parsing;

public class ParsingSceneContextTests
{

    [Fact]
    public void Initialization()
    {
        var sut = new ParsingSceneContext();
        sut.Should().BeEquivalentTo(new
        {
            HasMood = false,
            HasMusic = false
        });
    }

    [Fact]
    public void Reset()
    {
        var sut = new ParsingSceneContext();
        sut.HasMood = true;
        sut.HasMusic = true;

        sut.Should().BeEquivalentTo(new
        {
            HasMood = true,
            HasMusic = true
        });

        sut.Reset();

        sut.Should().BeEquivalentTo(new
        {
            HasMood = false,
            HasMusic = false
        });
    }
}
