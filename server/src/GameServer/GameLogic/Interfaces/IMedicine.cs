namespace GameServer.GameLogic;

public interface IMedicine
{
    /// <summary>
    /// Use the medicine.
    /// </summary>
    /// <param name="owner">Owner of the medicine.</param>
    public void Use(IPlayer owner);
}
