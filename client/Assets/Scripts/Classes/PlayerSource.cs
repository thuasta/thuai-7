using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor.MPE;
using UnityEditor.ShortcutManagement;
using UnityEngine;

public class PlayerSource: MonoBehaviour
{
    private static readonly Dictionary<int, Player> _playerDict = new();
    private static readonly GameObject playerPrefab = Resources.Load<GameObject>("Player/models/Soldier");
    private void Start()
    {
    }
    public static Dictionary<int ,Player> GetPlayers()
    {
        return _playerDict;
    }
    public static bool AddPlayer(int id, string name)
    {
        if (_playerDict.ContainsKey(id))
        {
            return false;
        }
        // Create Player obj
        GameObject newPlayerObj = Instantiate(playerPrefab);
        newPlayerObj.transform.position = Vector3.zero;

        _playerDict.Add(id, new Player {
                                    Id = id,
                                    Name = name,
                                    playerAnimations = newPlayerObj.GetComponent<PlayerAnimations>(),
                                    playerObj=newPlayerObj
        });

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
            player.UpdatePosition(position);
            player.TryGetPlayerAnimations();
        }
    }

    public static void Clear()
    {
        _playerDict.Clear();
    }
}
