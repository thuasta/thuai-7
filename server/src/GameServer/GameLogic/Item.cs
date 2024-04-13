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
            _ => throw new NotImplementedException()
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
