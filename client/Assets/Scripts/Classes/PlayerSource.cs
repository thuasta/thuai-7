using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor.MPE;
using UnityEditor.ShortcutManagement;

public class PlayerSource
{
    private static readonly Dictionary<string, Player> _playerDict = new();
    public static bool AddPlayer(Player player)
    {
        if (_playerDict.ContainsKey(player.Id))
        {
            return false;
        }
        _playerDict.Add(player.Id, player);
        return true;
    }

    public static void UpdatePlayer(Player player)
    {
        if (_playerDict.ContainsKey(player.Id))
        {
            _playerDict[player.Id] = player;
        }
        else
        {
            _playerDict.Add(player.Id, player);
        }
    }

    public static void Clear()
    {
        _playerDict.Clear();
    }
}
