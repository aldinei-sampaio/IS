namespace IS.Reading.Events;

public class MoodChangeEventTests
{
    [Theory]
    [InlineData("abc", MoodType.Normal, false, "mood normal: abc")]
    [InlineData("DEF", MoodType.Sad, true, "mood* sad: DEF")]
    [InlineData("Ghi", MoodType.Surprised, true, "mood* surprised: Ghi")]
    [InlineData("jKl", MoodType.Angry, false, "mood angry: jKl")]
    [InlineData("mnO", MoodType.Happy, true, "mood* happy: mnO")]
    public void Initialization(string personName, MoodType moodType, bool isProtagonist, string description)
    {
        var sut = new MoodChangeEvent(personName, isProtagonist, moodType);
        sut.PersonName.Should().Be(personName);
        sut.IsProtagonist.Should().Be(isProtagonist);
        sut.MoodType.Should().Be(moodType);
        sut.ToString().Should().Be(description);
    }
}
