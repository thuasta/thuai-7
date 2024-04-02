namespace GameServer.GameLogic;

public class PlayerAbandonEventArgs : EventArgs
{
    public const string EventName = "PlayerAbandon";
    public int PlayerId { get; }
    public int Number { get; }
    public List<string> AbandonedSupplies { get; }

    public PlayerAbandonEventArgs(int playerId, int number, List<string> abandonedSupplies)
    {
        PlayerId = playerId;
        Number = number;
        AbandonedSupplies = abandonedSupplies;
    }

}