namespace GameServer.GameLogic;

public class MedicineFactory
{
    /// <summary>
    /// Create a medicine from an item.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static IMedicine CreateFromItem(IItem item)
    {
        return item.ItemSpecificName switch
        {
            Constant.Names.BANDAGE => new Medicine(item.ItemSpecificName, Constant.BANDAGE_HEAL),
            Constant.Names.FIRST_AID => new Medicine(item.ItemSpecificName, Constant.FIRST_AID_HEAL),
            _ => throw new ArgumentException($"Item specific id {item.ItemSpecificName} is not valid for medicine."),
        };
    }

    /// <summary>
    /// Convert a medicine to an item.
    /// </summary>
    /// <param name="medicine"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static IItem ToItem(IMedicine medicine)
    {
        return new Item(IItem.ItemKind.Medicine, medicine.ItemSpecificName, 1);
    }
}

public class Medicine : IMedicine
{
    public string ItemSpecificName { get; }
    public int Heal { get; }
    public Medicine(string itemSpecificName, int heal)
    {
        ItemSpecificName = itemSpecificName;
        Heal = heal;
    }
}
