namespace IS.Reading.Parsing;

public static class Helper
{
    public static INodeParser FakeNodeParser(string name)
    {
        var parser = A.Fake<INodeParser>();
        A.CallTo(() => parser.Name).Returns(name);
        A.CallTo(() => parser.NodeAggregation).Returns(null);
        return parser;
    }

    public static T FakeParser<T>(string name) where T : class, IParser
    {
        var parser = A.Fake<T>();
        A.CallTo(() => parser.Name).Returns(name);
        return parser;
    }
}
