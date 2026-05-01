using Newtonsoft.Json;
using TattooToggler.Engine.Data;

namespace TattooToggler.IO.JSON
{
    internal static class SavedTattoosManager
    {
        private static readonly string SavePath = @"plugins/TattooToggler/SavedTattoos.json";

        // Root save file structure
        private class SaveFile
        {
            public SaveSlot Male { get; set; } = new();
            public SaveSlot Female { get; set; } = new();
        }

        // A single gender's save slot
        private class SaveSlot
        {
            public List<SavedDecoration> Tattoos { get; set; } = new();
        }

        // Minimal serializable representation of a Decoration
        private class SavedDecoration
        {
            public string CollectionName { get; set; }
            public uint CollectionHash { get; set; }
            public string OverlayName { get; set; }
            public uint OverlayHash { get; set; }
        }

        // --- Public API ---

        internal static void Save(List<Decoration> tattoos, Gender gender)
        {
            try
            {
                Normal($"[SavedTattoosManager] Saving {tattoos.Count} tattoos for gender: {gender}");

                SaveFile file = LoadFile();
                SaveSlot slot = GetSlot(file, gender);

                slot.Tattoos = tattoos.ConvertAll(t => new SavedDecoration
                {
                    CollectionName = t.CollectionName,
                    OverlayName = t.OverlayName,
                    OverlayHash = t.OverlayHash,
                });

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

                SaveFile file = LoadFile();
                SaveSlot slot = GetSlot(file, gender);

                if (slot.Tattoos.Count == 0)
                {
                    Normal($"[SavedTattoosManager] No saved tattoos found for gender: {gender}");
                    return new List<Decoration>();
                }

                // Resolve saved entries back against the master collection list
                // so we return full Decoration objects rather than the stripped-down saved version
                List<Decoration> resolved = new();
                List<Decoration> unresolved = new();

                foreach (SavedDecoration saved in slot.Tattoos)
                {
                    Decoration match = Collection.Collections
                        .SelectMany(c => c.Overlays)
                        .FirstOrDefault(d => d.OverlayHash == saved.OverlayHash);

                    if (match != null)
                        resolved.Add(match);
                    else
                        unresolved.Add(new Decoration(
                            overlayName: saved.OverlayName,
                            overlayHash: saved.OverlayHash,
                            gender: gender,
                            type: Type.TYPE_TATTOO,
                            zoneName: ZoneName.ZONE_TORSO,
                            garment: null,
                            price: 0,
                            awardLevel: 0,
                            collectionName: saved.CollectionName
                        ));
                }

                if (unresolved.Count > 0)
                    Normal(
                        $"[SavedTattoosManager] {unresolved.Count} saved tattoos could not be resolved against master list (addon tattoos or outdated save) — applying raw");

                resolved.AddRange(unresolved);
                Normal($"[SavedTattoosManager] Loaded {resolved.Count} tattoos for gender: {gender}");
                return resolved;
            }
            catch (Exception e)
            {
                Error(e);
                return new List<Decoration>();
            }
        }

        internal static bool HasSave(Gender gender)
        {
            try
            {
                SaveFile file = LoadFile();
                return GetSlot(file, gender).Tattoos.Count > 0;
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

        // --- Internals ---

        private static SaveSlot GetSlot(SaveFile file, Gender gender) =>
            gender == Gender.GENDER_MALE ? file.Male : file.Female;

        private static SaveFile LoadFile()
        {
            EnsureDirectory();

            if (!File.Exists(SavePath))
                return new SaveFile();

            string raw = File.ReadAllText(SavePath);

            if (string.IsNullOrWhiteSpace(raw))
                return new SaveFile();

            return JsonConvert.DeserializeObject<SaveFile>(raw) ?? new SaveFile();
        }

        private static void WriteFile(SaveFile file)
        {
            EnsureDirectory();
            File.WriteAllText(SavePath, JsonConvert.SerializeObject(file, Formatting.Indented));
        }

        private static void EnsureDirectory()
        {
            string dir = Path.GetDirectoryName(SavePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }
    }
}