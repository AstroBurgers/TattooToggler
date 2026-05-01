using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;
using TattooToggler.Engine.Data;

namespace TattooToggler.IO.JSON;

internal static class TattooDataParser
{
    internal static List<Collection> ParseFromFile(string jsonPath)
    {
        if (!File.Exists(jsonPath))
            throw new FileNotFoundException($"Tattoo data file not found: {jsonPath}");

        return ParseFromJson(File.ReadAllText(jsonPath));
    }

    private static List<Collection> ParseFromJson(string json)
    {
        List<Collection> result = new();
        Normal($"Beginning parse of JSON, {json.Length} chars");
        JArray root = JArray.Parse(json);
        Normal($"Found {root.Count} collections to parse");

        foreach (JToken collectionToken in root)
        {
            string collectionName = collectionToken["CollectionName"]?.ToString();
            uint collectionHash = collectionToken["CollectionHash"]?.Value<uint>() ?? 0;

            Normal($"Parsing collection: {collectionName}");

            List<Decoration> overlays = new();
            JArray overlayArray = collectionToken["Overlays"] as JArray;

            if (overlayArray == null) continue;

            Normal($"Collection '{collectionName}' has {overlayArray.Count} overlay entries");

            foreach (JToken item in overlayArray)
            {
                string overlayName = item["OverlayName"]?.ToString();
                uint overlayHash = item["OverlayHash"]?.Value<uint>() ?? 0;

                if (string.IsNullOrEmpty(overlayName)) continue;

                Decoration decoration = new(
                    overlayName: overlayName,
                    overlayHash: overlayHash,
                    gender: ParseGender(item["Gender"]?.ToString()),
                    type: ParseType(item["Type"]?.ToString()),
                    zoneName: ParseZone(item["Zone"]?.ToString()),
                    garment: item["Garment"]?.ToString(),
                    price: item["Price"]?.Value<int>() ?? 0,
                    awardLevel: item["AwardLevel"]?.Value<int>() ?? 0
                );

                Normal(
                    $"Adding decoration: Name={decoration.OverlayName}, Hash={decoration.OverlayHash}, Zone={decoration.ZoneName}, Gender={decoration.Gender}, Type={decoration.Type}, Price={decoration.Price}, AwardLevel={decoration.AwardLevel}, Garment={decoration.Garment ?? "N/A"}");
                overlays.Add(decoration);
            }
                
            // Now filter out anything that isn't actually a tattoo (e.g. badges) since we only want to toggle tattoos
            int beforeFilter = overlays.Count;
            overlays = overlays.Where(d => d.Type == Type.TYPE_TATTOO).ToList();
            int afterFilter = overlays.Count;
            Normal($"Filtered overlays for collection '{collectionName}': {beforeFilter} total, {afterFilter} tattoos remain");
                
                
            Normal($"Finished collection '{collectionName}' — {overlays.Count} decorations parsed");
            result.Add(new Collection(collectionName, collectionHash, overlays));
        }

        Normal(
            $"Parse complete. {result.Count} collections, {result.Sum(c => c.Overlays.Count)} total decorations");
        return result;
    }

    // --- Helpers ---

    private static Gender ParseGender(string genderStr)
    {
        if (string.IsNullOrEmpty(genderStr)) return Gender.GENDER_MALE;

        return Enum.TryParse(genderStr, ignoreCase: true, out Gender gender)
            ? gender
            : Gender.GENDER_MALE;
    }

    private static Type ParseType(string eFaction)
    {
        if (string.IsNullOrEmpty(eFaction)) return Type.TYPE_TATTOO;

        return eFaction.IndexOf("badge", StringComparison.OrdinalIgnoreCase) >= 0
            ? Type.TYPE_BADGE
            : Type.TYPE_TATTOO;
    }

    private static ZoneName ParseZone(string zoneStr)
    {
        if (string.IsNullOrEmpty(zoneStr)) return ZoneName.ZONE_TORSO;

        return Enum.TryParse(zoneStr, ignoreCase: true, out ZoneName zone)
            ? zone
            : ZoneName.ZONE_TORSO;
    }

    // Jenkins one-at-a-time hash — matches GTA's GET_HASH_KEY output
    private static uint Jenkins(string input)
    {
        if (string.IsNullOrEmpty(input)) return 0;

        string lower = input.ToLowerInvariant();
        uint hash = 0;

        foreach (char c in lower)
        {
            hash += (byte)c;
            hash += hash << 10;
            hash ^= hash >> 6;
        }

        hash += hash << 3;
        hash ^= hash >> 11;
        hash += hash << 15;

        return hash;
    }
}