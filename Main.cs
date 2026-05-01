using TattooToggler.IO.JSON;
using static TattooToggler.Engine.Data.Collection;

[assembly: Rage.Attributes.Plugin("Tattoo Toggler", Description = "Someone say irreversible decisions?", Author = "Astro")]

namespace TattooToggler;

public class EntryPoint
{
    internal static Ped MainPlayer => Game.LocalPlayer.Character;
    
    
    internal static void Main()
    {
        try
        {
            Game.DisplayNotification("go fuck yourself :)");
            Collections = IO.JSON.TattooDataParser.ParseFromFile(@"plugins/TattooToggler/TattooData/TattooData.json");
            Game.DisplayNotification($"Parsed: {Collections}");
            Engine.UI.MainMenu.CreateMenu();
            
            // Apply saved tattoos
            List<Decoration> saved = SavedTattoosManager.Load(Engine.UI.MainMenu.GetPlayerGender());

            foreach (Decoration tattoo in saved)
            {
                MainPlayer.AddTattoo(Game.GetHashKey(tattoo.CollectionName), Game.GetHashKey(tattoo.OverlayName));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}