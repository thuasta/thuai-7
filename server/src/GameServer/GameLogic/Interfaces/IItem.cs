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

    public static ItemKind GetItemKind(string itemName)
    {
        return itemName switch
        {
            Constant.Names.AWM => ItemKind.Weapon,
            Constant.Names.S686 => ItemKind.Weapon,
            Constant.Names.M16 => ItemKind.Weapon,
            Constant.Names.VECTOR => ItemKind.Weapon,
            Constant.Names.GRENADE => ItemKind.Grenade,
            Constant.Names.BULLET => ItemKind.Bullet,
            Constant.Names.FIRST_AID => ItemKind.Medicine,
            Constant.Names.BANDAGE => ItemKind.Medicine,
            Constant.Names.PRIMARY_ARMOR => ItemKind.Armor,
            Constant.Names.PREMIUM_ARMOR => ItemKind.Armor,
            _ => throw new ArgumentException($"Unknown item: {itemName}.")
        };
    }

    /// <summary>
    /// Kind of the item.
    /// </summary>
    public ItemKind Kind { get; }

    /// <summary>
    /// Specific id of the item.
    /// </summary>
    public string ItemSpecificName { get; }

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

    /// <summary>
    /// Additional properties of the item.
    /// </summary>
    public object? AdditionalProperties { get; init; }
}
