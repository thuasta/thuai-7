using System.Numerics;

namespace GameServer.GameLogic;

public interface IBackPack
{
    /// <summary>
    /// Capacity of the backpack.
    /// </summary>
    public int Capacity { get; }

    /// <summary>
    /// Current weight of the backpack.
    /// </summary>
    public int CurrentWeight { get; }

    /// <summary>
    /// Items in the backpack.
    /// </summary>
    public List<IItem> Items { get; }

    /// <summary>
    /// Add items to the backpack.
    /// </summary>
    /// <param name="kind"></param>
    /// <param name="itemSpecificName"></param>
    /// <param name="count"></param>
    public void AddItems(IItem.ItemKind kind, string itemSpecificName, int count);

    /// <summary>
    /// Remove items from the backpack.
    /// </summary>
    /// <param name="kind"></param>
    /// <param name="itemSpecificName"></param>
    /// <param name="count"></param>
    public void RemoveItems(IItem.ItemKind kind, string itemSpecificName, int count);

    /// <summary>
    /// Remove items from the backpack��return it's count
    /// </summary>
    /// <param name="kind"></param>
    /// <param name="itemSpecificName"></param>
    public IItem? FindItems(IItem.ItemKind kind, string itemSpecificName);
}
