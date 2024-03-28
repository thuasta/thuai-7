namespace GameServer.GameLogic;

public interface IPlayer
{
    int Health { get; set; }
    double Speed { get; set; }
    Armor? PlayerArmor { get; set; }
    IWeapon PlayerWeapon { get; set; }
    IBackPack PlayerBackPack { get; set; }
    Position PlayerPosition { get; set; }
    void TakeDamage(int v);
    void TakeHeal(int v);
}
