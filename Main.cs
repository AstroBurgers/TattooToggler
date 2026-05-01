using System.Windows.Forms;
using TattooToggler.Engine.Data;
using TattooToggler.IO.JSON;
using static TattooToggler.Engine.Data.Collection;

[assembly: Rage.Attributes.Plugin("Tattoo Toggler", Description = "Someone say irreversible decisions?", Author = "Astro")]

namespace TattooToggler;

public class EntryPoint
{
    internal static Ped MainPlayer => Game.LocalPlayer.Character;


    internal static void Main()
    {
        Normal("=== TattooToggler initializing ===");

        try
        {
            Notify("Loading TattooToggler...", true);

            // 1. Tattoo data
            Normal("Parsing tattoo data...");
            Collections = TattooDataParser.ParseFromFile(@"plugins/TattooToggler/TattooData/TattooData.json");

            if (Collections == null)
                throw new Exception("Tattoo collections failed to load.");

            Normal($"Tattoo data loaded. Collections: {Collections.Count}");

            // 2. Config
            Normal("Loading INI...");
            IO.Configuration.INIFile.LoadIni();
            Normal("INI loaded.");

            // 3. UI
            Normal("Creating menu...");
            Engine.UI.MainMenu.CreateMenu();
            Normal("Menu created.");

            // 4. Saved tattoos
            Gender gender = Engine.UI.MainMenu.GetPlayerGender();
            Normal($"Detected player gender: {gender}");

            List<Decoration> saved = SavedTattoosManager.Load(gender);
            Engine.UI.MainMenu.CurrentTattoos = saved; // Set current tattoos to the loaded ones so the menu can reflect them
            Normal($"Applying {saved.Count} saved tattoos...");

            foreach (Decoration tattoo in saved)
            {
                MainPlayer.AddTattoo(
                    Game.GetHashKey(tattoo.CollectionName),
                    Game.GetHashKey(tattoo.OverlayName)
                );
            }

            Notify("TattooToggler loaded successfully.", true);
            Normal("=== TattooToggler initialized successfully ===");
        }
        catch (Exception e)
        {
            Error(e);
            Notify("TattooToggler failed to load. Check log.", false);
        }
    }

    private static void Notify(string message, bool success)
    {
        string prefix = success ? "~g~TattooToggler~s~" : "~r~TattooToggler~s~";
        Game.DisplayNotification($"{prefix}: {message}");
    }
}