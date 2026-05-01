using System.Windows.Forms;
using static TattooToggler.IO.Configuration.INIFile;

namespace TattooToggler.Engine.Helpers;

internal static class KeybindHelpers
{
    internal static bool AreMenuKeysPressed()
    {
        return (ModifierKey == Keys.None || Game.IsKeyDownRightNow(ModifierKey))
               && Game.IsKeyDownRightNow(MenuKey);
    }
}