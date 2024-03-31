namespace GameServer.GameLogic;

public class Item : IItem
{
    public IItem.ItemKind Kind { get; }
    public string ItemSpecificName { get; }
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
    /// <param name="itemSpecificName"></param>
    /// <param name="count"></param>
    public Item(IItem.ItemKind kind, string itemSpecificName, int count)
    {
        Kind = kind;
        ItemSpecificName = itemSpecificName;
        Count = count;
    }
}
