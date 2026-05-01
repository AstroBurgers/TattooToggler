using Rage.Native;

namespace TattooToggler.Engine.Helpers;

internal static class TattooHelpers
{
    internal static void AddTattoo(this Ped ped, uint collectionNameHash, uint overlayNameHash)
    {
        NativeFunction.Natives.x5F5D1665E352A839(ped, collectionNameHash, overlayNameHash); // ADD_PED_DECORATION_FROM_HASHES
    }

    internal static void ClearTattoos(this Ped ped)
    {
        NativeFunction.Natives.xE3B27E70CEAB9F0C(ped); // CLEAR_PED_DECORATIONS_LEAVE_SCARS
    }
}