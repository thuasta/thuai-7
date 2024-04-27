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

    public void GenerateItems(IItem item)
    {
        if (IsWall)
        {
            throw new InvalidOperationException("Cannot generate items in a wall");
        }

        if (IItem.AllowPileUp(item.Kind) == true)
        {
            // If the item can be piled up, add the count to the existing item
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].Kind == item.Kind && Items[i].ItemSpecificName == item.ItemSpecificName)
                {
                    Items[i].Count += item.Count;
                    return;
                }
            }
        }

        Items.Add(item);
    }

    public void RemoveItems(IItem item)
    {
        if (IsWall)
        {
            throw new InvalidOperationException("Cannot remove items from a wall");
        }

        if (item.AdditionalProperties is null)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].Kind == item.Kind && Items[i].ItemSpecificName == item.ItemSpecificName)
                {
                    if (Items[i].Count < item.Count)
                    {
                        throw new ArgumentException($"No enough items: {item.ItemSpecificName}");
                    }

                    Items[i].Count -= item.Count;
                    if (Items[i].Count == 0)
                    {
                        Items.RemoveAt(i);
                    }
                    return;
                }
            }
            throw new ArgumentException($"Item {item.ItemSpecificName} not found.");
        }
        else
        {
            switch (item.AdditionalProperties)
            {
                case ArmorProperties armorProperties:
                    for (int i = 0; i < Items.Count; i++)
                    {
                        if (Items[i].AdditionalProperties is null
                            || Items[i].AdditionalProperties is not ArmorProperties)
                        {
                            continue;
                        }

                        if (Items[i].Kind == item.Kind && Items[i].ItemSpecificName == item.ItemSpecificName
                            && (Items[i].AdditionalProperties as ArmorProperties)?.CurrentHealth
                                == armorProperties.CurrentHealth)
                        {
                            if (Items[i].Count < item.Count)
                            {
                                throw new ArgumentException($"No enough items: {item.ItemSpecificName}");
                            }

                            Items[i].Count -= item.Count;
                            if (Items[i].Count == 0)
                            {
                                Items.RemoveAt(i);
                            }
                            return;
                        }
                    }
                    throw new ArgumentException(
                        $"Item {item.ItemSpecificName} with health {armorProperties.CurrentHealth} not found."
                    );

                case WeaponProperties weaponProperties:
                    for (int i = 0; i < Items.Count; i++)
                    {
                        if (Items[i].AdditionalProperties is null
                            || Items[i].AdditionalProperties is not WeaponProperties)
                        {
                            continue;
                        }

                        if (Items[i].Kind == item.Kind && Items[i].ItemSpecificName == item.ItemSpecificName
                            && (Items[i].AdditionalProperties as WeaponProperties)?.TicksUntilAvailable
                                == weaponProperties.TicksUntilAvailable)
                        {
                            if (Items[i].Count < item.Count)
                            {
                                throw new ArgumentException($"No enough items: {item.ItemSpecificName}");
                            }

                            Items[i].Count -= item.Count;
                            if (Items[i].Count == 0)
                            {
                                Items.RemoveAt(i);
                            }
                            return;
                        }
                    }
                    throw new ArgumentException(
                        $"Item {item.ItemSpecificName} with property {weaponProperties.TicksUntilAvailable} not found."
                    );

                default:
                    throw new ArgumentException(
                        $"Additional properties {item.AdditionalProperties.GetType().Name} is not valid for item."
                    );
            }
        }
    }
}
