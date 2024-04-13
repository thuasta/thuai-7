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

    public void AddItems(IItem.ItemKind kind, string itemSpecificName, int count)
    {
        if (CurrentWeight + new Item(kind, itemSpecificName, count).Weight > Capacity)
        {
            throw new InvalidOperationException($"No enough capacity for the item {itemSpecificName}");
        }

        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].Kind == kind && Items[i].ItemSpecificName == itemSpecificName)
            {
                Items[i].Count += count;
                return;
            }
        }
        // Inventory does not contain the item
        Items.Add(new Item(kind, itemSpecificName, count));
    }

    public void RemoveItems(IItem.ItemKind kind, string itemSpecificName, int count)
    {

        if (!Items.Any(item => item.Kind == kind && item.ItemSpecificName == itemSpecificName))
        {
            throw new ArgumentException($"Item {itemSpecificName} not found");
        }

        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].Kind == kind && Items[i].ItemSpecificName == itemSpecificName)
            {
                if (Items[i].Count < count)
                {
                    throw new ArgumentException($"No enough items: {itemSpecificName}");
                }

                Items[i].Count -= count;
                if (Items[i].Count == 0)
                {
                    Items.RemoveAt(i);
                }
            }
        }
    }

    public IItem? FindItems(IItem.ItemKind kind, string itemSpecificName)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].Kind == kind && Items[i].ItemSpecificName == itemSpecificName)
            {
                return Items[i];
            }
        }
        return null;
    }
}
