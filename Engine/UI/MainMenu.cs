using System.Linq;
using RAGENativeUI;
using RAGENativeUI.Elements;
using TattooToggler.Engine.Data;

namespace TattooToggler.Engine.UI;

internal static class MainMenu
{
    #region TattooData

    internal static List<Decoration> CurrentTattoos { get; set; } = [];
    internal static Decoration SelectedTattoo { get; set; }

    internal static List<Decoration> HeadTattoos { get; set; } = [];
    internal static List<Decoration> TorsoTattoos { get; set; } = [];
    internal static List<Decoration> LeftArmTattoos { get; set; } = [];
    internal static List<Decoration> RightArmTattoos { get; set; } = [];
    internal static List<Decoration> LeftLegTattoos { get; set; } = [];
    internal static List<Decoration> RightLegTattoos { get; set; } = [];

    private static Dictionary<ZoneName, List<Decoration>> GetTattoosByZone(List<Collection> collections)
    {
        List<Decoration> tattoos = collections
            .SelectMany(c => c.Overlays)
            .Where(d => d.Type == Type.TYPE_TATTOO)
            .ToList();

        return new Dictionary<ZoneName, List<Decoration>>
        {
            { ZoneName.ZONE_HEAD, tattoos.Where(d => d.ZoneName == ZoneName.ZONE_HEAD).ToList() },
            { ZoneName.ZONE_TORSO, tattoos.Where(d => d.ZoneName == ZoneName.ZONE_TORSO).ToList() },
            { ZoneName.ZONE_LEFT_ARM, tattoos.Where(d => d.ZoneName == ZoneName.ZONE_LEFT_ARM).ToList() },
            { ZoneName.ZONE_RIGHT_ARM, tattoos.Where(d => d.ZoneName == ZoneName.ZONE_RIGHT_ARM).ToList() },
            { ZoneName.ZONE_LEFT_LEG, tattoos.Where(d => d.ZoneName == ZoneName.ZONE_LEFT_LEG).ToList() },
            { ZoneName.ZONE_RIGHT_LEG, tattoos.Where(d => d.ZoneName == ZoneName.ZONE_RIGHT_LEG).ToList() },
        };
    }

    private static void LoadTattoosByZone(List<Collection> collections)
    {
        Dictionary<ZoneName, List<Decoration>> tattoosByZone = GetTattoosByZone(collections);
        HeadTattoos = tattoosByZone[ZoneName.ZONE_HEAD];
        TorsoTattoos = tattoosByZone[ZoneName.ZONE_TORSO];
        LeftArmTattoos = tattoosByZone[ZoneName.ZONE_LEFT_ARM];
        RightArmTattoos = tattoosByZone[ZoneName.ZONE_RIGHT_ARM];
        LeftLegTattoos = tattoosByZone[ZoneName.ZONE_LEFT_LEG];
        RightLegTattoos = tattoosByZone[ZoneName.ZONE_RIGHT_LEG];
    }

    #endregion

    internal static readonly UIMenuNumericScrollerItem<int> HeadZoneScroller = new("Head Tattoos", "Select a tattoo to make it permanent with enter, scroll to browse.", 0, HeadTattoos.Count, 1);
    internal static readonly UIMenuNumericScrollerItem<int> TorsoZoneScroller = new("Torso Tattoos", "Select a tattoo to make it permanent with enter, scroll to browse.", 0, TorsoTattoos.Count, 1);
    internal static readonly UIMenuNumericScrollerItem<int> LeftArmZoneScroller = new("Left Arm Tattoos", "Select a tattoo to make it permanent with enter, scroll to browse.", 0, LeftArmTattoos.Count, 1);
    internal static readonly UIMenuNumericScrollerItem<int> RightArmZoneScroller = new("Right Arm Tattoos", "Select a tattoo to make it permanent with enter, scroll to browse.", 0, RightArmTattoos.Count, 1);
    internal static readonly UIMenuNumericScrollerItem<int> LeftLegZoneScroller = new("Left Leg Tattoos", "Select a tattoo to make it permanent with enter, scroll to browse.", 0, LeftLegTattoos.Count, 1);
    internal static readonly UIMenuNumericScrollerItem<int> RightLegZoneScroller = new("Right Leg Tattoos", "Select a tattoo to make it permanent with enter, scroll to browse.", 0, RightLegTattoos.Count, 1);
    
    internal static readonly UIMenuItem RemoveAllTattoosItem = new("Remove All Tattoos", "Remove all tattoos from your character");
    internal static readonly UIMenuItem SaveTattoosItem = new("Save Current Tattoos", "Save your current tattoos to reapply them later");
    
    internal static readonly UIMenu MainUIMenu = new("Tattoo Toggler", "Select a tattoo to toggle it on/off");
    internal static readonly MenuPool MainMenuPool = new();

    internal static void CreateMenu()
    {
        Normal("Creating main menu...");
        LoadTattoosByZone(Collection.Collections);
        MainMenuPool.Add(MainUIMenu);

        MainUIMenu.MouseControlsEnabled = false;
        MainUIMenu.AllowCameraMovement = true;
        
        MainUIMenu.AddItems();
    }
}