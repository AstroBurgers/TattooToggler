namespace TattooToggler.Engine.Data;

internal class Collection(string collectionName, uint collectionHash, List<Decoration> overlays)
{
    internal static List<Collection> Collections { get; set; } = new();

    internal string CollectionName { get; set; } = collectionName;
    internal uint CollectionHash { get; set; } = collectionHash;

    internal List<Decoration> Overlays { get; set; } = overlays;
}