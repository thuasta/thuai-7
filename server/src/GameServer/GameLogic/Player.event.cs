using GameServer.Geometry;
using static GameServer.GameLogic.IItem;

namespace GameServer.GameLogic;

public partial class Player
{
    public enum PlayerEventType
    {
        PlayerAbandon,
        PlayerAttack,
        PlayerPickUp,
        PlayerSwitchArm,
        PlayerUseGrenade,
        PlayerUseMedicine,
        PlayerTeleport
    };

    public class PlayerAbandonEventArgs : EventArgs
    {
        public const PlayerEventType EventName = PlayerEventType.PlayerAbandon;
        public Player Player { get; }
        public int Number { get; }
        public (ItemKind ItemKind, string ItemSpecificName) AbandonedSupplies { get; }

        public PlayerAbandonEventArgs(Player player, int number, (ItemKind itemKind, string itemSpecificName) abandonedSupplies)
        {
            Player = player;
            Number = number;
            AbandonedSupplies = abandonedSupplies;
        }
    }

    public class PlayerAttackEventArgs : EventArgs
    {
        public const PlayerEventType EventName = PlayerEventType.PlayerAttack;
        public Player Player { get; }
        public Position TargetPosition { get; }

        public PlayerAttackEventArgs(Player player, Position targetPosition)
        {
            Player = player;
            TargetPosition = targetPosition;
        }
    }

    public class PlayerPickUpEventArgs : EventArgs
    {
        public const PlayerEventType EventName = PlayerEventType.PlayerPickUp;
        public Player Player { get; }
        public string TargetSupply { get; }
        public Position TargetPosition { get; }
        public int Numb { get; }

        public PlayerPickUpEventArgs(Player player, string targetSupply, Position targetPosition, int numb)
        {
            Player = player;
            TargetSupply = targetSupply;
            TargetPosition = targetPosition;
            Numb = numb;
        }
    }

    public class PlayerSwitchArmEventArgs : EventArgs
    {
        public const PlayerEventType EventName = PlayerEventType.PlayerSwitchArm;
        public Player Player { get; }
        public string TargetFirearm { get; }

        public PlayerSwitchArmEventArgs(Player player, string targetFirearm)
        {
            Player = player;
            TargetFirearm = targetFirearm;
        }
    }

    public class PlayerTeleportEventArgs : EventArgs
    {
        public const PlayerEventType EventName = PlayerEventType.PlayerTeleport;
        public Player Player { get; }
        public Position TargetPosition { get; }

        public PlayerTeleportEventArgs(Player player, Position targetPosition)
        {
            Player = player;
            TargetPosition = targetPosition;
        }
    }

    public class PlayerUseGrenadeEventArgs : EventArgs
    {
        public const PlayerEventType EventName = PlayerEventType.PlayerUseGrenade;
        public Player Player { get; }
        public Position TargetPosition { get; }

        public PlayerUseGrenadeEventArgs(Player player, Position targetPosition)
        {
            Player = player;
            TargetPosition = targetPosition;
        }
    }

    public class PlayerUseMedicineEventArgs : EventArgs
    {
        public const PlayerEventType EventName = PlayerEventType.PlayerUseMedicine;
        public Player Player { get; }
        public string MedicineName { get; }

        public PlayerUseMedicineEventArgs(Player player, string medicineName)
        {
            Player = player;
            MedicineName = medicineName;
        }
    }

    public event EventHandler<PlayerAbandonEventArgs>? PlayerAbandonEvent;
    public event EventHandler<PlayerAttackEventArgs>? PlayerAttackEvent;
    public event EventHandler<PlayerPickUpEventArgs>? PlayerPickUpEvent;
    public event EventHandler<PlayerSwitchArmEventArgs>? PlayerSwitchArmEvent;
    public event EventHandler<PlayerTeleportEventArgs>? PlayerTeleportEvent;
    public event EventHandler<PlayerUseGrenadeEventArgs>? PlayerUseGrenadeEvent;
    public event EventHandler<PlayerUseMedicineEventArgs>? PlayerUseMedicineEvent;
}
