using System.Collections.Generic;
using TMPro;
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
    public class FaceCamera : MonoBehaviour
    {
        private void LateUpdate()
        {
            transform.forward = GameObject.Find("Camera").transform.forward;
        }
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
        //newBeam.transform.SetParent(newPlayerObj.transform);
        newBeam.transform.position = new Vector3(0, 1.8f, 0);
        // Create Player
        GameObject uiCanvasGO = new GameObject("PlayerIDCanvas");
        uiCanvasGO.transform.SetParent(newPlayerObj.transform); // Set the canvas as a child of the player object
        uiCanvasGO.transform.localPosition = new Vector3(0, 3.0f, 0); // Adjust as needed for positioning
        Canvas canvas = uiCanvasGO.AddComponent<Canvas>();
        canvas.sortingLayerID = LayerMask.NameToLayer("UI");
        canvas.renderMode = RenderMode.WorldSpace; // Set the render mode to World Space
        TextMeshProUGUI textComponent = uiCanvasGO.AddComponent<TextMeshProUGUI>();
        textComponent.text = "Player ID: " + id; // Set the initial text to the player's ID
        textComponent.fontSize = 1; // Set the font size
        textComponent.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        textComponent.color = Color.red; // Set the font color
        textComponent.alignment = TextAlignmentOptions.Center;
        textComponent.rectTransform.pivot = new Vector2(0.5f, 0.5f); // Ensure text is centered
        canvas.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
        // Ensure the UI text always faces the camera
        uiCanvasGO.AddComponent<FaceCamera>();
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

    public static void UpdatePlayer(int id, int health, ArmorTypes armor, float speed, FirearmTypes firearm, Dictionary<string, int> inventory, Position position,float firearmRange)
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
            player.FirearmRange = firearmRange;
        }
    }

    public static void Clear()
    {
        _playerDict.Clear();
    }
}
