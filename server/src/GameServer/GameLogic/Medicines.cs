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
        throw new NotImplementedException();
    }

    /// <summary>
    /// Convert a medicine to an item.
    /// </summary>
    /// <param name="medicine"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static IItem ToItem(IMedicine medicine)
    {
        throw new NotImplementedException();
    }
}

public class Medicine : IMedicine
{
    public Medicine()
    {
        // Do nothing
    }

    public void Use(IPlayer owner)
    {
        owner.TakeHeal(Constant.MEDICINE_HEAL);
    }
}
