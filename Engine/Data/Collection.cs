using MessagePack;

namespace TattooToggler.Engine.Data;

[MessagePackObject]
public class Collection
{
    [Key(0)] public string CollectionName { get; set; }

    [Key(1)] public uint CollectionHash { get; set; }

    [Key(2)] public List<Decoration> Overlays { get; set; }

    // This is runtime state, not serialized.
    [IgnoreMember] public static List<Collection> Collections { get; set; } = [];

    public Collection()
    {
        CollectionName = string.Empty;
        Overlays = [];
    }

    public Collection(
        string collectionName,
        uint collectionHash,
        List<Decoration> overlays)
    {
        CollectionName = collectionName;
        CollectionHash = collectionHash;
        Overlays = overlays;
    }
}