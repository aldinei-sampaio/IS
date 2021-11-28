using IS.Reading.State;

namespace IS.Reading.Nodes;

internal static class ExtensionMethods
{
    public static bool IsProtagonist(this INavigationState state)
    {
        if (state.Person is null || state.Protagonist is null)
            return false;
        return state.Person == state.Protagonist;
    }
}
