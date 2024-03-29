namespace GameServer.GameLogic;

public class Item : IItem
{
    public IItem.ItemKind Kind { get; }
    public int ItemSpecificId { get; }
    public int Count { get; set; }
    public int WeightOfSingleItem
    {
        get => Kind switch
        {
            _ => throw new NotImplementedException()
        };
    }
    public int Weight
    {
        get => Count * WeightOfSingleItem;
    }

    /// <summary>
    /// Constructor for an item.
    /// </summary>
    /// <param name="kind"></param>
    /// <param name="itemSpecificId"></param>
    /// <param name="count"></param>
    public Item(IItem.ItemKind kind, int itemSpecificId, int count)
    {
        Kind = kind;
        ItemSpecificId = itemSpecificId;
        Count = count;
    }
}
