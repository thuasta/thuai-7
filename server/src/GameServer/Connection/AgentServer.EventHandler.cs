using GameServer.GameController;
using GameServer.GameLogic;

namespace GameServer.Connection;

public partial class AgentServer
{
    public void HandleAfterGameTickEvent(object? sender, AfterGameTickEventArgs e)
    {
        List<MapMessage.Wall> walls = new();
        List<SuppliesMessage.Supply> supplies = new();

        // Add walls and supplies
        for (int x = 0; x < e.Game.GameMap.Width; x++)
        {
            for (int y = 0; y < e.Game.GameMap.Height; y++)
            {
                // Add walls
                if (e.Game.GameMap.MapChunk[x, y].IsWall == true)
                {
                    walls.Add(
                        new MapMessage.Wall
                        {
                            Position = new MapMessage.Wall.WallPositions
                            {
                                X = x,
                                Y = y
                            }
                        }
                    );
                }

                // Add supplies
                foreach (IItem item in e.Game.GameMap.MapChunk[x, y].Items)
                {
                    supplies.Add(
                        new SuppliesMessage.Supply
                        {
                            Name = item.ItemSpecificName,
                            PositionOfSupply = new SuppliesMessage.Supply.Position
                            {
                                X = x,
                                Y = y
                            },
                        }
                    );
                }
            }
        }

        List<PlayersInfoMessage.Player> players = new();

        // Add players
        foreach (Player player in e.Game.AllPlayers)
        {
            List<PlayersInfoMessage.Player.Item> inventory = new();

            // Add inventory
            foreach (IItem item in player.PlayerBackPack.Items)
            {
                inventory.Add(
                    new PlayersInfoMessage.Player.Item
                    {
                        Name = item.ItemSpecificName,
                        Num = item.Count
                    }
                );
            }

            // Add player
            players.Add(
                new PlayersInfoMessage.Player
                {
                    PlayerId = player.PlayerId,
                    Armor = (player.PlayerArmor is not null) ? player.PlayerArmor.ItemSpecificName : "NO_ARMOR",
                    Health = player.Health,
                    Speed = player.Speed,
                    Firearm = new PlayersInfoMessage.Player.FirearmInfo
                    {
                        Name = player.PlayerWeapon.Name,
                        Distance = player.PlayerWeapon.Range
                    },
                    Position = new PlayersInfoMessage.Player.PositionInfo
                    {
                        X = player.PlayerPosition.x,
                        Y = player.PlayerPosition.y
                    },
                    Inventory = new(inventory)
                }
            );
        }

        // Append map message, supplies message, players info message, and safe zone message to _messageToPublish
        _messageToPublish.Enqueue(
            new MapMessage
            {
                Length = e.Game.GameMap.Height,
                Walls = walls
            }
        );
        _messageToPublish.Enqueue(
            new SuppliesMessage
            {
                Supplies = new(supplies)
            }
        );
        _messageToPublish.Enqueue(
            new PlayersInfoMessage
            {
                Players = new(players)
            }
        );
        _messageToPublish.Enqueue(
            new SafeZoneMessage
            {
                CenterOfCircle = new SafeZoneMessage.Center
                {
                    X = e.Game.GameMap.SafeZone.Center.x,
                    Y = e.Game.GameMap.SafeZone.Center.y
                },
                Radius = e.Game.GameMap.SafeZone.Radius
            }
        );
    }

    public void HandleAfterNewPlayerJoinEvent(object? sender, AfterNewPlayerJoinEventArgs e)
    {
        _socketTokens.TryAdd(e.SocketId, e.Token);
        Publish(
            new PlayerIdMessage() { PlayerId = e.PlayerId },
            e.Token
        );
    }
}
