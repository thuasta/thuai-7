namespace GameServer.GameLogic;

/// <summary>
/// Interface for armor.
/// </summary>
public interface IArmor
{
    /// <summary>
    /// Id of the armor corresponding to the item.
    /// </summary>
    public int ItemSpecificId { get; }

    /// <summary>
    /// Health of the armor.
    /// </summary>
    public int Health { get; }

    /// <summary>
    /// Maximum health of the armor.
    /// </summary>
    public int MaxHealth { get; }

    /// <summary>
    /// Deal some damage.
    /// </summary>
    /// <param name="Damage"></param>
    /// <returns>Damage dealt by player</returns>
    public int Hurt(int Damage);
}
