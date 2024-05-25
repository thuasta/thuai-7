using System.Collections.Generic;

using UnityEngine;
using static Thubg.Messages.CompetitionUpdate;

using TMPro;

public class Player
{
    public int Id;
    public string Name;
    public int Health;
    public ArmorTypes Armor;
    public float Speed;
    public FirearmTypes Firearm;
    public Dictionary<string, int> Inventory;
    public Position PlayerPosition;
    public PlayerAnimations playerAnimations;
    public BeamAnimations beamAnimations;
    public GameObject playerObj;
    public GameObject beam;
    public GameObject playerTitle;
    public GameObject uiCanvasGo;
    public Color playerColor;
    public float FirearmRange;

    public bool IsDead { get; private set; } = false;

    public class FaceCamera : MonoBehaviour
    {
        private GameObject _camera;
        private void LateUpdate()
        {
            if(_camera == null)
            {
                _camera = GameObject.Find("Camera");
            }
            transform.forward = _camera.transform.forward;
        }
    }

    public Player(int id, string name, Color color, GameObject playerPrefab, GameObject beamPrefab)
    {
        Id = id;
        Name = name;
        playerColor = color;
        CreatePlayerObj(playerPrefab);
        CreateBeam(beamPrefab);
        CreateTitleUI();
    }


    public void TryGetPlayerAnimations()
    {
        if (playerAnimations == null)
        {
            playerAnimations = playerObj.GetComponent<PlayerAnimations>();
        }
    }

    public void Attack(Position targetPosition, float range)
    {
        if(IsDead) return;
        FaceTo(targetPosition);
        ShowGunFire(targetPosition, range);
        playerAnimations.SetFiring();
    }

    public void UseMedicine()
    {
        if(IsDead) return;
        playerAnimations.SetDrinking();
    }

    public void Die(Color deadColor)
    {
        playerAnimations.Stop();
        if (IsDead) return;
        playerAnimations.SetDead();
        IsDead = true;
        playerColor = deadColor;
        UpdateUiColor();
    }

    public void FaceTo(Position pos)
    {
        if (pos.x == playerObj.transform.position.x && pos.y == playerObj.transform.position.z)
        {
            return;
        }
        Vector3 direction = new Vector3(pos.x, playerObj.transform.position.y, pos.y) - playerObj.transform.position;
        playerObj.transform.forward = Quaternion.AngleAxis(30, Vector3.up) * direction;
    }

    public void ShowGunFire(Position pos, float range)
    {
        Vector3 height = new Vector3(0, 1.8f, 0);
        Vector3 endPoint = new Vector3(pos.x, playerObj.transform.position.y, pos.y) + height;
        beam.transform.position = playerObj.transform.position + height ;
        MeshRenderer childComponent = beam.GetComponentInChildren<MeshRenderer>();
        childComponent.transform.localPosition = new Vector3(childComponent.transform.localPosition.x, childComponent.transform.localPosition.y, range/2);
        childComponent.transform.localScale = new Vector3(childComponent.transform.localScale.x, childComponent.transform.localScale.y, range);

        beam.transform.LookAt(endPoint);
        beamAnimations.Blink(PlayerAnimations.AttackTime);
    }

    public void UpdatePosition(Position pos)
    {
        if(IsDead) return;
        PlayerPosition = pos;
        // Compute Delta
        Vector3 newPos = new Vector3(pos.x, playerObj.transform.position.y, pos.y);
        FaceTo(pos);
        TryGetPlayerAnimations();
        playerAnimations?.WalkTo(playerObj.transform.position, newPos);
        playerObj.transform.position = newPos;
        //Debug.Log(newPos);
    }
    public void CreateTitleUI()
    {
        uiCanvasGo = new GameObject("PlayerIDCanvas");
        uiCanvasGo.transform.SetParent(playerObj.transform); // Set the canvas as a child of the player object
        uiCanvasGo.transform.localPosition = new Vector3(0, 3.0f, 0); // Adjust as needed for positioning
        Canvas canvas = uiCanvasGo.AddComponent<Canvas>();
        canvas.sortingLayerID = LayerMask.NameToLayer("UI");
        canvas.renderMode = RenderMode.WorldSpace; // Set the render mode to World Space
        TextMeshProUGUI textComponent = uiCanvasGo.AddComponent<TextMeshProUGUI>();
        textComponent.text = "" + (Name.Length <= 6 ? Name : Name.Substring(0, 6) + "..."); // Set the initial text to the player's token
        textComponent.fontSize = 2; // Set the font size
        textComponent.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        textComponent.color = playerColor; // Set the font color
        textComponent.alignment = TextAlignmentOptions.Center;
        textComponent.rectTransform.pivot = new Vector2(0.5f, 0.5f); // Ensure text is centered
        canvas.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
        // Ensure the UI text always faces the camera
        uiCanvasGo.AddComponent<FaceCamera>();
    }

    public void UpdateUiColor()
    {
        uiCanvasGo.GetComponent<TextMeshProUGUI>().color = playerColor;
    }

    public void CreatePlayerObj(GameObject playerPrefab)
    {
        playerObj = GameObject.Instantiate(playerPrefab);
        playerObj.transform.position = Vector3.zero;
    }

    public void CreateBeam(GameObject beamPrefab)
    {
        beam = GameObject.Instantiate(beamPrefab);
        beam.SetActive(false);
        beam.GetComponentInChildren<MeshRenderer>().material.color = playerColor;
        beamAnimations = beam.GetComponent<BeamAnimations>();
        playerAnimations = playerObj.GetComponent<PlayerAnimations>();
    }
}
