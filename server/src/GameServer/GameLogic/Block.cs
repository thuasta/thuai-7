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

    public void GenerateItems(IItem.ItemKind kind, int itemSpecificId, int count)
    {
        if (IsWall)
        {
            throw new InvalidOperationException("Cannot generate items in a wall");
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
        if (IsWall)
        {
            throw new InvalidOperationException("Cannot remove items from a wall");
        }

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
}
