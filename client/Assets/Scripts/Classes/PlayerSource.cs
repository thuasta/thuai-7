using System.Collections.Generic;
using UnityEngine;

public class PlayerSource : MonoBehaviour
{
    private static readonly Dictionary<int, Player> _playerDict = new();
    private static readonly GameObject playerPrefab = Resources.Load<GameObject>("Player/models/Soldier");
    private static readonly GameObject beamPrefab = Resources.Load<GameObject>("Beam/Beam");
    private void Start()
    {
    }
    public static Dictionary<int, Player> GetPlayers()
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

        GameObject newBeam = Instantiate(beamPrefab);
        newBeam.SetActive(false);
        if (_playerDict.Count == 0)
            newBeam.GetComponentInChildren<MeshRenderer>().material.color = Color.blue;
        else
            newBeam.GetComponentInChildren<MeshRenderer>().material.color = Color.red;
        newBeam.transform.SetParent(newPlayerObj.transform);
        newBeam.transform.localPosition = new Vector3(0, 1.8f, 0);
        // Create Player
        
        _playerDict.Add(id, new Player
        {
            Id = id,
            Name = name,
            playerAnimations = newPlayerObj.GetComponent<PlayerAnimations>(),
            playerObj = newPlayerObj,
            beam = newBeam,
            beamAnimations = newBeam.GetComponent<BeamAnimations>()
        });

        return true;
    }

    public static void UpdatePlayer(int id, int health, ArmorTypes armor, float speed, FirearmTypes firearm, Dictionary<string, int> inventory, Position position)
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
