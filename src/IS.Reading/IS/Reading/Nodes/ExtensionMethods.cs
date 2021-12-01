﻿using IS.Reading.State;

namespace IS.Reading.Nodes;

internal static class ExtensionMethods
{
    public static bool IsProtagonist(this INavigationState state)
    {
        if (state.PersonName is null || state.ProtagonistName is null)
            return false;
        return state.PersonName == state.ProtagonistName;
    }
}
