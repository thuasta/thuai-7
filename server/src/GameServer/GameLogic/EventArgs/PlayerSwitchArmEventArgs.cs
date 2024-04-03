namespace GameServer.GameLogic;

public class PlayerSwitchArmEventArgs : EventArgs
{
    public const string EventName = "PlayerSwitchArm";
    public int PlayerId { get; }
    public string TargetFirearm { get; }

    public PlayerSwitchArmEventArgs(int playerId, string targetFirearm)
    {
        PlayerId = playerId;
        TargetFirearm = targetFirearm;
    }

}