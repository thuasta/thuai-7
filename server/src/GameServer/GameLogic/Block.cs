namespace GameServer.GameLogic;

public class Block : IBlock
{
    public bool IsWall { get; }
    public List<IItem> Items { get; }

    public Block(bool isWall)
    {
        IsWall = isWall;
        Items = new();
    }

    public void GenerateItems(IItem.ItemKind kind, string itemSpecificName, int count)
    {
        if (IsWall)
        {
            throw new InvalidOperationException("Cannot generate items in a wall");
        }

        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].Kind == kind && Items[i].ItemSpecificName == itemSpecificName)
            {
                Items[i].Count += count;
                return;
            }
        }

        Items.Add(new Item(kind, itemSpecificName, count));
    }

    public void RemoveItems(IItem.ItemKind kind, string itemSpecificName, int count)
    {
        if (IsWall)
        {
            throw new InvalidOperationException("Cannot remove items from a wall");
        }

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
}
