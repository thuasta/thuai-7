namespace GameServer.GameLogic;

public interface IMedicine
{
    /// <summary>
    /// Use the medicine.
    /// </summary>
    /// <param name="owner">Owner of the medicine.</param>

    public string ItemSpecificName { get; }

    public int Heal { get; }
}
