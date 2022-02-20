using IS.Reading.State;

namespace IS.Reading.Nodes;

internal static class ExtensionMethods
{
    public static bool IsMainCharacter(this INavigationState state)
    {
        if (state.PersonName is null || state.MainCharacterName is null)
            return false;
        return state.PersonName == state.MainCharacterName;
    }
}
