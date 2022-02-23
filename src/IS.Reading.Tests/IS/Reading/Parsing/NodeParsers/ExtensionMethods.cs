using FakeItEasy.Core;
using IS.Reading.Choices;
using IS.Reading.Parsing.NodeParsers.ChoiceParsers;

namespace IS.Reading.Parsing.NodeParsers;

internal static class ExtensionMethods
{
    public static BuilderParentParsingContext<string> GetTestContext(this IFakeObjectCall fakeObjectCall)
        => GetCtx<string>(fakeObjectCall);

    public static BuilderParentParsingContext<IChoicePrototype> GetChoiceContext(this IFakeObjectCall fakeObjectCall)
        => GetCtx<IChoicePrototype>(fakeObjectCall);

    public static BuilderParentParsingContext<IChoiceOptionPrototype> GetOptionContext(this IFakeObjectCall fakeObjectCall)
    => GetCtx<IChoiceOptionPrototype>(fakeObjectCall);

    private static BuilderParentParsingContext<T> GetCtx<T>(IFakeObjectCall fakeObjectCall)
        => fakeObjectCall.GetArgument<BuilderParentParsingContext<T>>(2);
}
