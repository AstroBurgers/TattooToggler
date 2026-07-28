using MessagePack;

namespace TattooToggler.Engine.Data;

public enum ZoneName
{
    ZONE_TORSO = 0,
    ZONE_HEAD = 1,
    ZONE_LEFT_ARM = 2,
    ZONE_RIGHT_ARM = 3,
    ZONE_LEFT_LEG = 4,
    ZONE_RIGHT_LEG = 5,
    ZONE_MEDALS = 6,
    ZONE_INVALID = 7
}

public enum Gender
{
    GENDER_MALE = 0,
    GENDER_FEMALE = 1
}

public enum DecorationType
{
    TYPE_TATTOO = 0,
    TYPE_BADGE = 1
}

[MessagePackObject]
public class Decoration
{
    [Key(0)] public string OverlayName { get; set; }

    [Key(1)] public uint OverlayHash { get; set; }

    [Key(2)] public Gender Gender { get; set; }

    [Key(3)] public DecorationType Type { get; set; }

    [Key(4)] public ZoneName ZoneName { get; set; }

    [Key(5)] public string CollectionName { get; set; }

    public Decoration(
        string overlayName,
        uint overlayHash,
        Gender gender,
        DecorationType type,
        ZoneName zoneName,
        string collectionName)
    {
        OverlayName = overlayName;
        OverlayHash = overlayHash;
        Gender = gender;
        Type = type;
        ZoneName = zoneName;
        CollectionName = collectionName;
    }
}