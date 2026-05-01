internal enum ZoneName
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

internal enum Gender
{
    GENDER_MALE = 0,
    GENDER_FEMALE = 1
}

internal enum Type
{
    TYPE_TATTOO = 0,
    TYPE_BADGE = 1
}

namespace TattooToggler.Engine.Data
{
    internal class Decoration
    {
        public Decoration(string overlayName, uint overlayHash, Gender gender, Type type, ZoneName zoneName, string garment, int price, int awardLevel, string collectionName)
        {
            OverlayName = overlayName;
            OverlayHash = overlayHash;
            Gender = gender;
            Type = type;
            ZoneName = zoneName;
            Garment = garment;
            Price = price;
            AwardLevel = awardLevel;
            CollectionName = collectionName;
        }

        internal string OverlayName { get; set; }
        internal uint OverlayHash { get; set; }
        
        internal Gender Gender { get; set; }
        internal Type Type { get; set; }
        internal ZoneName ZoneName { get; set; }
        
        internal string Garment { get; set; }
        internal int Price { get; set; }
        internal int AwardLevel { get; set; }
        internal string CollectionName { get; set; }
    }
}