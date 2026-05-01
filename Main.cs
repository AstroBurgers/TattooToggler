using static TattooToggler.Engine.Data.Collection;

[assembly: Rage.Attributes.Plugin("Tattoo Toggler", Description = "Someone say irreversible decisions?", Author = "Astro")]

namespace TattooToggler;

public class EntryPoint
{
    internal static void Main()
    {
        try
        {
            Game.DisplayNotification("go fuck yourself :)");
            Collections = IO.JSON.TattooDataParser.ParseFromFile(@"plugins/TattooToggler/TattooData/TattooData.json");
            Game.DisplayNotification($"Parsed: {Collections}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}