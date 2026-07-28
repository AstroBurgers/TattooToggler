#nullable enable
using MessagePack;
using TattooToggler.Engine.Data;

namespace TattooToggler.IO.Parsing;

public static class SavedTattoosManager
{
    private const string SavePath = @"plugins/TattooToggler/SavedTattoos.dat";

    [MessagePackObject]
    public class SaveFile
    {
        [Key(0)] public SaveSlot Male { get; set; } = new();

        [Key(1)] public SaveSlot Female { get; set; } = new();
    }

    [MessagePackObject]
    public class SaveSlot
    {
        [Key(0)] public List<SavedDecoration> Tattoos { get; set; } = [];
    }

    [MessagePackObject]
    public class SavedDecoration
    {
        [Key(0)] public string OverlayName { get; set; } = "";

        [Key(1)] public uint OverlayHash { get; set; }

        [Key(2)] public string CollectionName { get; set; } = "";
    }

    internal static void Save(List<Decoration> tattoos, Gender gender)
    {
        try
        {
            Normal($"[SavedTattoosManager] Saving {tattoos.Count} tattoos for gender: {gender}");

            SaveFile file = LoadFile();
            SaveSlot slot = GetSlot(file, gender);

            slot.Tattoos = tattoos.Select(x => new SavedDecoration
            {
                OverlayName = x.OverlayName,
                OverlayHash = x.OverlayHash,
                CollectionName = x.CollectionName
            }).ToList();

            WriteFile(file);

            Normal($"[SavedTattoosManager] Save successful for gender: {gender}");
        }
        catch (Exception e)
        {
            Error(e);
        }
    }

    internal static List<Decoration> Load(Gender gender)
    {
        try
        {
            Normal($"[SavedTattoosManager] Loading saved tattoos for gender: {gender}");

            SaveSlot slot = GetSlot(LoadFile(), gender);

            if (slot.Tattoos.Count == 0)
                return [];

            List<Decoration> resolved = [];

            foreach (SavedDecoration saved in slot.Tattoos)
            {
                Decoration? match = Collection.Collections
                    .SelectMany(c => c.Overlays)
                    .FirstOrDefault(d => d.OverlayHash == saved.OverlayHash);

                if (match != null)
                {
                    resolved.Add(match);
                    continue;
                }

                Normal($"[SavedTattoosManager] Could not resolve {saved.OverlayName}, loading raw");

                resolved.Add(new Decoration(
                    overlayName: saved.OverlayName,
                    overlayHash: saved.OverlayHash,
                    gender: gender,
                    type: DecorationType.TYPE_TATTOO,
                    zoneName: ZoneName.ZONE_TORSO,
                    collectionName: saved.CollectionName
                ));
            }

            Normal($"[SavedTattoosManager] Loaded {resolved.Count} tattoos for gender: {gender}");

            return resolved;
        }
        catch (Exception e)
        {
            Error(e);
            return [];
        }
    }

    internal static bool HasSave(Gender gender)
    {
        try
        {
            return GetSlot(LoadFile(), gender).Tattoos.Count > 0;
        }
        catch
        {
            return false;
        }
    }

    internal static void ClearSlot(Gender gender)
    {
        try
        {
            Normal($"[SavedTattoosManager] Clearing save slot for gender: {gender}");

            SaveFile file = LoadFile();
            GetSlot(file, gender).Tattoos.Clear();

            WriteFile(file);
        }
        catch (Exception e)
        {
            Error(e);
        }
    }

    internal static void ClearAll()
    {
        try
        {
            Normal("[SavedTattoosManager] Clearing all save slots");
            WriteFile(new SaveFile());
        }
        catch (Exception e)
        {
            Error(e);
        }
    }

    private static SaveSlot GetSlot(SaveFile file, Gender gender) =>
        gender == Gender.GENDER_MALE ? file.Male : file.Female;

    private static SaveFile LoadFile()
    {
        EnsureDirectory();

        if (!File.Exists(SavePath))
            return new SaveFile();

        byte[] data = File.ReadAllBytes(SavePath);

        return data.Length == 0 ? new SaveFile() : MessagePackSerializer.Deserialize<SaveFile>(data);
    }

    private static void WriteFile(SaveFile file)
    {
        EnsureDirectory();

        byte[] data = MessagePackSerializer.Serialize(file);

        File.WriteAllBytes(SavePath, data);
    }

    private static void EnsureDirectory()
    {
        string? dir = Path.GetDirectoryName(SavePath);

        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
    }
}