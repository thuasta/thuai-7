namespace GameServer.GameLogic;

public class Item : IItem
{
    public IItem.ItemKind Kind { get; }
    public string ItemSpecificName { get; }
    public int Count { get; set; }

    public int WeightOfSingleItem
    {
        get => ItemSpecificName switch
        {
            Constant.Names.VECTOR => Constant.Weights.VECTOR,
            Constant.Names.AWM => Constant.Weights.AWM,
            Constant.Names.S686 => Constant.Weights.S686,
            Constant.Names.M16 => Constant.Weights.M16,
            Constant.Names.BULLET => Constant.Weights.BULLET,
            Constant.Names.FIRST_AID => Constant.Weights.FIRST_AID,
            Constant.Names.BANDAGE => Constant.Weights.BANDAGE,
            Constant.Names.GRENADE => Constant.Weights.GRENADE,
            Constant.Names.PRIMARY_ARMOR => Constant.Weights.PRIMARY_ARMOR,
            Constant.Names.PREMIUM_ARMOR => Constant.Weights.PREMIUM_ARMOR,
            _ => throw new ArgumentException($"Unknown item: {ItemSpecificName}.")
        };
    }
    public int Weight
    {
        get => Count * WeightOfSingleItem;
    }

    public object? AdditionalProperties { get; init; } = null;

    /// <summary>
    /// Constructor for an item.
    /// </summary>
    /// <param name="kind"></param>
    /// <param name="itemSpecificName"></param>
    /// <param name="count"></param>
    public Item(IItem.ItemKind kind, string itemSpecificName, int count)
    {
        if (count <= 0)
        {
            throw new ArgumentException("Count must be positive.");
        }
        if (IItem.AllowPileUp(kind) == false && count > 1)
        {
            throw new ArgumentException($"Item kind {kind} does not allow piling up.");
        }

        Kind = kind;
        ItemSpecificName = itemSpecificName;
        Count = count;
    }
}
