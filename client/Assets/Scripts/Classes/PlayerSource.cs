using System.Collections.Generic;
using UnityEngine;

public class PlayerSource
{
    private static readonly Dictionary<int, Player> _playerDict = new();
    public static bool AddPlayer(int id, string name)
    {
        if (_playerDict.ContainsKey(id))
        {
            return false;
        }
        _playerDict.Add(id, new Player {
                                    Id = id,
                                    Name = name,
                                    playerAnimations = GameObject.Find(name).GetComponent<PlayerAnimations>()});
        return true;
    }

    public static void UpdatePlayer(int id, int health, ArmorTypes armor, float speed, FirearmTypes firearm, Dictionary<Items, int> inventory, Position position)
    {
        if (_playerDict.ContainsKey(id))
        {
            Player player = _playerDict[id];
            player.Health = health;
            player.Armor = armor;
            player.Speed = speed;
            player.Firearm = firearm;
            player.Inventory = inventory;
            player.PlayerPosition = position;
            player.TryGetPlayerAnimations();
        }
    }

    public static void Clear()
    {
        _playerDict.Clear();
    }
}
