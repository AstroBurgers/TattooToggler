using System.Drawing;
using System.Windows.Forms;
using RAGENativeUI;
using RAGENativeUI.Elements;
using RAGENativeUI.PauseMenu;
using TattooToggler.IO.JSON;
using static TattooToggler.EntryPoint;

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

    private static Dictionary<ZoneName, List<Decoration>> GetTattoosByZone(List<Collection> collections, Gender gender)
    {
        List<Decoration> tattoos = collections
            .SelectMany(c => c.Overlays)
            .Where(d => d.Type == Type.TYPE_TATTOO && d.Gender == gender)
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
        Gender playerGender = GetPlayerGender();
        Normal($"Loading tattoos for gender: {playerGender}");

        Dictionary<ZoneName, List<Decoration>> tattoosByZone = GetTattoosByZone(collections, playerGender);
        HeadTattoos = tattoosByZone[ZoneName.ZONE_HEAD];
        TorsoTattoos = tattoosByZone[ZoneName.ZONE_TORSO];
        LeftArmTattoos = tattoosByZone[ZoneName.ZONE_LEFT_ARM];
        RightArmTattoos = tattoosByZone[ZoneName.ZONE_RIGHT_ARM];
        LeftLegTattoos = tattoosByZone[ZoneName.ZONE_LEFT_LEG];
        RightLegTattoos = tattoosByZone[ZoneName.ZONE_RIGHT_LEG];
    }

    internal static Gender GetPlayerGender()
    {
        uint modelHash = (uint)Game.LocalPlayer.Character.Model.Hash;
        uint maleHash = (uint)Game.GetHashKey("MP_M_FREEMODE_01");

        return modelHash == maleHash ? Gender.GENDER_MALE : Gender.GENDER_FEMALE;
    }
    
    #endregion

    internal static Gender LastPlayerGender { get; set; }
    
    internal static UIMenuListScrollerItem<string> HeadZoneScroller { get; private set; }
    internal static UIMenuListScrollerItem<string> TorsoZoneScroller { get; private set; }
    internal static UIMenuListScrollerItem<string> LeftArmZoneScroller { get; private set; }
    internal static UIMenuListScrollerItem<string> RightArmZoneScroller { get; private set; }
    internal static UIMenuListScrollerItem<string> LeftLegZoneScroller { get; private set; }
    internal static UIMenuListScrollerItem<string> RightLegZoneScroller { get; private set; }

    internal static readonly UIMenuItem RemoveAllTattoosItem =
        new("Remove All Tattoos", "Remove all tattoos from your character");

    internal static readonly UIMenuItem SaveTattoosItem =
        new("Save Current Tattoos", "Save your current tattoos to reapply them later");

    internal static readonly UIMenuItem ClearSavedTattoosItem =
        new("Clear Saved Tattoos", "Clear your saved tattoos for your current gender");
    
    internal static readonly UIMenuItem ApplySavedTattoosItem =
        new("Apply Saved Tattoos", "Apply your saved tattoos for your current gender");
    
    internal static readonly UIMenu MainUiMenu = new("Tattoo Toggler", "Select a tattoo to toggle it on/off");
    internal static readonly MenuPool MainMenuPool = new();

    internal static void CreateMenu()
    {
        Normal("Creating main menu...");
        LoadTattoosByZone(Collection.Collections);

        HeadZoneScroller = new UIMenuListScrollerItem<string>("Head Tattoos",
            "Select a tattoo to make it permanent with enter, select it again to remove it, scroll to browse.",
            HeadTattoos.Select(t => t.OverlayName).ToList());
        TorsoZoneScroller = new UIMenuListScrollerItem<string>("Torso Tattoos",
            "Select a tattoo to make it permanent with enter, select it again to remove it, scroll to browse.",
            TorsoTattoos.Select(t => t.OverlayName).ToList());
        LeftArmZoneScroller = new UIMenuListScrollerItem<string>("Left Arm Tattoos",
            "Select a tattoo to make it permanent with enter, select it again to remove it, scroll to browse.",
            LeftArmTattoos.Select(t => t.OverlayName).ToList());
        RightArmZoneScroller = new UIMenuListScrollerItem<string>("Right Arm Tattoos",
            "Select a tattoo to make it permanent with enter, select it again to remove it, scroll to browse.",
            RightArmTattoos.Select(t => t.OverlayName).ToList());
        LeftLegZoneScroller = new UIMenuListScrollerItem<string>("Left Leg Tattoos",
            "Select a tattoo to make it permanent with enter, select it again to remove it, scroll to browse.",
            LeftLegTattoos.Select(t => t.OverlayName).ToList());
        RightLegZoneScroller = new UIMenuListScrollerItem<string>("Right Leg Tattoos",
            "Select a tattoo to make it permanent with enter, select it again to remove it, scroll to browse.",
            RightLegTattoos.Select(t => t.OverlayName).ToList());

        LastPlayerGender = GetPlayerGender();
        
        MainMenuPool.Add(MainUiMenu);

        MainUiMenu.MouseControlsEnabled = false;
        MainUiMenu.AllowCameraMovement = true;

        MainUiMenu.AddItems(HeadZoneScroller, TorsoZoneScroller, LeftArmZoneScroller, RightArmZoneScroller,
            LeftLegZoneScroller, RightLegZoneScroller, ApplySavedTattoosItem, SaveTattoosItem, RemoveAllTattoosItem, ClearSavedTattoosItem);

        ApplySavedTattoosItem.BackColor = Color.DarkBlue;
        SaveTattoosItem.BackColor = Color.DarkGreen;
        RemoveAllTattoosItem.BackColor = Color.DarkRed;
        ClearSavedTattoosItem.BackColor = Color.DarkRed;
        
        HeadZoneScroller.IndexChanged += HeadZoneScrollerOnIndexChanged;
        HeadZoneScroller.Activated += HeadZoneScrollerOnActivated;

        TorsoZoneScroller.IndexChanged += TorsoZoneScrollerOnIndexChanged;
        TorsoZoneScroller.Activated += TorsoZoneScrollerOnActivated;

        LeftArmZoneScroller.IndexChanged += LeftArmZoneScrollerOnIndexChanged;
        LeftArmZoneScroller.Activated += LeftArmZoneScrollerOnActivated;

        RightArmZoneScroller.IndexChanged += RightArmZoneScrollerOnIndexChanged;
        RightArmZoneScroller.Activated += RightArmZoneScrollerOnActivated;

        LeftLegZoneScroller.IndexChanged += LeftLegZoneScrollerOnIndexChanged;
        LeftLegZoneScroller.Activated += LeftLegZoneScrollerOnActivated;

        RightLegZoneScroller.IndexChanged += RightLegZoneScrollerOnIndexChanged;
        RightLegZoneScroller.Activated += RightLegZoneScrollerOnActivated;

        RemoveAllTattoosItem.Activated += (_, _) =>
        {
            CurrentTattoos.Clear();
            RefreshTattoos();
        };

        SaveTattoosItem.Activated += (_, _) =>
        {
            SavedTattoosManager.Save(CurrentTattoos, GetPlayerGender());
            Game.DisplayNotification($"Tattoos saved for {GetPlayerGender()}.");
        };

        ClearSavedTattoosItem.Activated += (_, _) =>
        {
            SavedTattoosManager.ClearSlot(LastPlayerGender);
        };
        
        ApplySavedTattoosItem.Activated += (_, _) =>
        {
            List<Decoration> saved = SavedTattoosManager.Load(GetPlayerGender());
            if (saved.Count == 0)
            {
                Game.DisplayNotification($"No saved tattoos found for {GetPlayerGender()}.");
                return;
            }
            CurrentTattoos = saved;
            RefreshTattoos();
        };
        
        GameFiber.StartNew(MenuPoolProcess);
    }

    private static void CycleTattoo(int index, ZoneName zoneName)
    {
        RefreshTattoos();
        
        SelectedTattoo = zoneName switch
        {
            ZoneName.ZONE_HEAD => HeadTattoos.ElementAtOrDefault(index),
            ZoneName.ZONE_TORSO => TorsoTattoos.ElementAtOrDefault(index),
            ZoneName.ZONE_LEFT_ARM => LeftArmTattoos.ElementAtOrDefault(index),
            ZoneName.ZONE_RIGHT_ARM => RightArmTattoos.ElementAtOrDefault(index),
            ZoneName.ZONE_LEFT_LEG => LeftLegTattoos.ElementAtOrDefault(index),
            ZoneName.ZONE_RIGHT_LEG => RightLegTattoos.ElementAtOrDefault(index),
            _ => null
        };

        if (CurrentTattoos.Contains(SelectedTattoo))
        {
            return;
        }

        if (SelectedTattoo == null) return;
        MainPlayer.AddTattoo(Game.GetHashKey(SelectedTattoo.CollectionName), Game.GetHashKey(SelectedTattoo.OverlayName));
    }

    private static void AddTattoo()
    {
        if (SelectedTattoo == null) return;
        if (CurrentTattoos.Contains(SelectedTattoo))
        {
            CurrentTattoos.Remove(SelectedTattoo);
            RefreshTattoos();
            return;
        }
        CurrentTattoos.Add(SelectedTattoo);
    }

    private static void RefreshTattoos()
    {
        MainPlayer.ClearTattoos();
        foreach (Decoration tattoo in CurrentTattoos)
        {
            MainPlayer.AddTattoo(Game.GetHashKey(tattoo.CollectionName), Game.GetHashKey(tattoo.OverlayName));
        }
    }
    
    #region Handlers

    private static void RightLegZoneScrollerOnActivated(UIMenu sender, UIMenuItem selectedItem)
    {
        AddTattoo();
    }

    private static void RightLegZoneScrollerOnIndexChanged(UIMenuScrollerItem sender, int oldIndex, int newIndex)
    {
        CycleTattoo(newIndex, ZoneName.ZONE_RIGHT_LEG);
    }

    private static void LeftLegZoneScrollerOnActivated(UIMenu sender, UIMenuItem selectedItem)
    {
        AddTattoo();
    }

    private static void LeftLegZoneScrollerOnIndexChanged(UIMenuScrollerItem sender, int oldIndex, int newIndex)
    {
        CycleTattoo(newIndex, ZoneName.ZONE_LEFT_LEG);
    }

    private static void RightArmZoneScrollerOnActivated(UIMenu sender, UIMenuItem selectedItem)
    {
        AddTattoo();
    }

    private static void RightArmZoneScrollerOnIndexChanged(UIMenuScrollerItem sender, int oldIndex, int newIndex)
    {
        CycleTattoo(newIndex, ZoneName.ZONE_RIGHT_ARM);
    }

    private static void LeftArmZoneScrollerOnActivated(UIMenu sender, UIMenuItem selectedItem)
    {
        AddTattoo();
    }

    private static void LeftArmZoneScrollerOnIndexChanged(UIMenuScrollerItem sender, int oldIndex, int newIndex)
    {
        CycleTattoo(newIndex, ZoneName.ZONE_LEFT_ARM);
    }

    private static void TorsoZoneScrollerOnActivated(UIMenu sender, UIMenuItem selectedItem)
    {
        AddTattoo();
    }

    private static void TorsoZoneScrollerOnIndexChanged(UIMenuScrollerItem sender, int oldIndex, int newIndex)
    {
        CycleTattoo(newIndex, ZoneName.ZONE_TORSO);
    }

    private static void HeadZoneScrollerOnActivated(UIMenu sender, UIMenuItem selectedItem)
    {
        AddTattoo();
    }

    private static void HeadZoneScrollerOnIndexChanged(UIMenuScrollerItem sender, int oldIndex, int newIndex)
    {
        CycleTattoo(newIndex, ZoneName.ZONE_HEAD);
    }

    #endregion


    private static void MenuPoolProcess()
    {
        try
        {
            while (true)
            {
                GameFiber.Yield();
                MainMenuPool.ProcessMenus();
                if (!Game.IsKeyDown(Keys.K))
                    continue; // If the button defined in the INI Is pressed trigger the IF State meant
                if (MenuRequirements()) // Checking menu requirements defined below
                {
                    if (GetPlayerGender() != LastPlayerGender)
                    {
                        LastPlayerGender = GetPlayerGender();
                        LoadTattoosByZone(Collection.Collections);
                    }
                    MainUiMenu.Visible = true; // Making the menu visible
                }
                else if (MainUiMenu.Visible)
                {
                    MainUiMenu.CurrentItem.Selected = false;

                    MainUiMenu.Visible = false; // Making the menu no longer visible
                }
            }
        }
        catch (Exception e)
        { 
            Error(e);
        }
    }

    private static bool MenuRequirements() // The aforementioned menu requirements
    {
        return
            !UIMenu.IsAnyMenuVisible &&
            !TabView.IsAnyPauseMenuVisible; // Makes sure that the player is not paused/in a Compulite style menu. Checks if any other menus are open
    }
}