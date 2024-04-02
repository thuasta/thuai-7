namespace GameServer.GameLogic;

public class PlayerPickUpEventArgs : EventArgs
{
    public const string EventName = "PlayerPickUp";
    public int PlayerId { get; }
    public string TargetSupply { get; }
    public Position TargetPosition { get; }

    public PlayerPickUpEventArgs(int playerId, string targetSupply, Position targetPosition)
    {
        PlayerId = playerId;
        TargetSupply = targetSupply;
        TargetPosition = targetPosition;
    }

}