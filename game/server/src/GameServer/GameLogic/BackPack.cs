namespace GameServer.GameLogic;

public class BackPack : IBackPack
{
    public int Capacity { get; }
    public int CurrentWeight
    {
        get
        {
            int currentWeight = 0;
            foreach (IItem item in Items)
            {
                currentWeight += item.Count * item.WeightOfSingleItem;
            }
            return currentWeight;
        }
    }
    public List<IItem> Items { get; }

    /// <summary>
    /// Constructor of the backpack.
    /// </summary>
    /// <param name="capacity">Capacity of the backpack</param>
    public BackPack(int capacity)
    {
        Capacity = capacity;
        Items = new();
    }

    public void AddItems(IItem.ItemKind kind, int itemSpecificId, int count)
    {
        if (CurrentWeight + new Item(kind, itemSpecificId, count).Weight > Capacity)
        {
            throw new InvalidOperationException($"No enough capacity for the item {itemSpecificId}");
        }

        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].Kind == kind && Items[i].ItemSpecificId == itemSpecificId)
            {
                Items[i].Count += count;
                return;
            }
        }

        Items.Add(new Item(kind, itemSpecificId, count));
    }

    public void RemoveItems(IItem.ItemKind kind, int itemSpecificId, int count)
    {

        if (!Items.Any(item => item.Kind == kind && item.ItemSpecificId == itemSpecificId))
        {
            throw new ArgumentException($"Item {itemSpecificId} not found");
        }

        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].Kind == kind && Items[i].ItemSpecificId == itemSpecificId)
            {
                if (Items[i].Count < count)
                {
                    throw new ArgumentException($"No enough items: {itemSpecificId}");
                }

                Items[i].Count -= count;
                if (Items[i].Count == 0)
                {
                    Items.RemoveAt(i);
                }
            }
        }
    }

    public int FindItems(IItem.ItemKind kind, int itemSpecificId)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].Kind == kind && Items[i].ItemSpecificId == itemSpecificId)
            {
                return Items[i].Count;
            }
        }
        return 0;
    }
}
