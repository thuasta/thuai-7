namespace GameServer.GameLogic;

public interface IPlayer
{
    int Health { get; set; }

    /// <summary>
    /// Speed per tick
    /// </summary>
    double Speed { get; set; }

    /// <summary>
    /// Player Target Position, used for player to move to a target position, null if player is not moving.
    /// </summary>
    public Position? PlayerTargetPosition { get; set; }
    Armor? PlayerArmor { get; set; }
    IWeapon PlayerWeapon { get; set; }
    IBackPack PlayerBackPack { get; set; }
    Position PlayerPosition { get; set; }
    void TakeDamage(int v);
    void TakeHeal(int v);
}
