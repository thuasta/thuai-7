namespace GameServer.GameLogic;

public interface IItem
{
    public enum ItemKind
    {
        Medicine,
        Weapon,
        Bullet,
        Grenade,
        Armor
    }

    /// <summary>
    /// Kind of the item.
    /// </summary>
    public ItemKind Kind { get; }

    /// <summary>
    /// Specific id of the item.
    /// </summary>
    public int ItemSpecificId { get; }

    /// <summary>
    /// Count of the item.
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Weight of a single item.
    /// </summary>
    public int WeightOfSingleItem { get; }

    /// <summary>
    /// Weight of the item in total.
    /// </summary>
    public int Weight { get; }
}
