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
        return new Medicine(item.ItemSpecificId);
    }

    /// <summary>
    /// Convert a medicine to an item.
    /// </summary>
    /// <param name="medicine"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static IItem ToItem(IMedicine medicine)
    {
        return new Item(IItem.ItemKind.Medicine, medicine.ItemSpecificId, 1);
    }
}

public class Medicine : IMedicine
{
    public int ItemSpecificId { get; }
    public Medicine(int itemSpecificId)
    {
        ItemSpecificId = itemSpecificId;
    }

    public void Use(IPlayer owner)
    {
        owner.TakeHeal(Constant.MEDICINE_HEAL);
    }
}
