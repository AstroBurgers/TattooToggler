using System.Windows.Forms;

namespace TattooToggler.IO.Configuration;

internal static class INIFile
{
    internal static Keys MenuKey { get; private set; } = Keys.K;
    internal static Keys ModifierKey { get; private set; } = Keys.LShiftKey;

    internal static InitializationFile Inifile; // Defining a new INI File

    internal static void LoadIni()
    {
        Inifile = new InitializationFile(@"plugins/TattooToggler/Config.ini");
        Inifile.Create();
        
        MenuKey = Inifile.ReadEnum("General", "MenuKey", MenuKey);
        ModifierKey = Inifile.ReadEnum("General", "ModifierKey", ModifierKey);
    }
}