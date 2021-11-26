namespace IS.Reading.Parsing;

public static class Helper
{
    public static T FakeParser<T>(string name) where T : class, IParser
    {
        var parser = A.Fake<T>();
        A.CallTo(() => parser.Name).Returns(name);
        return parser;
    }
}
